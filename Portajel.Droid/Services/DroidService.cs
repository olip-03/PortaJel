using Android.Content;
using Microsoft.Maui.Controls;
using Portajel.Connections.Services.Database;
using PortaJel.Droid.Services;
using Portajel.Structures.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Portajel.Connections.Interfaces;
using Portajel.Connections;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Portajel.Services;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Android.Opengl;
using System.IO;
using System.Transactions;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using Portajel.Droid.Playback;
using Resource = Microsoft.Maui.Resource;

namespace Portajel.Droid.Services
{
    [Service(Name = "PortaJel.Droid.ServiceController", IsolatedProcess = true, ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
    public class DroidService : Service
    {
        const string ACTION_NOTIFICATION_TAPPED = "com.portajel.ACTION_NOTIFICATION_TAPPED";
        const string ACTION_NOTIFICATION_DELETED = "com.portajel.ACTION_NOTIFICATION_DELETED";
        
        // Add these fields to your class
        private NotificationManager? _notificationManager;
        private const int NOTIFICATION_ID = 517;
        private const string CHANNEL_ID = "sync_channel";
        private bool isSyncRunning = false;
        public DroidServiceBinder Binder { get; set; } = default!;
        public DroidMediaController MediaController { get; set; } = new();
        public DatabaseConnector database { get; private set; } = null!;
        public ServerConnector serverConnector { get; private set; } = null!;
        public DroidService()
        {
            
        }
        public override IBinder? OnBind(Intent? intent)
        {
            Initialize();
            try
            {
                _ = Task.Run(async () =>
                {
                    await serverConnector.AuthenticateAsync();
                    await serverConnector.StartSyncAsync();
                    if (serverConnector != null)
                    {
                        await SaveHelper.SaveData(serverConnector);
                    }
                });
            }
            catch (Exception ex)
            {
                throw;
            };

            Binder = new DroidServiceBinder(this);
            return Binder;
        }
        
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            MediaController.Initialize();
            
            var action = intent?.Action;
            if (action == "ACTION_NOTIFICATION_DISMISSED")
            {
                isSyncRunning = false;
                return StartCommandResult.NotSticky;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    CHANNEL_ID,
                    "PortaJel Data Sync Channel",
                    NotificationImportance.Low);
                
                _notificationManager = (NotificationManager?)GetSystemService(NotificationService);
                _notificationManager?.CreateNotificationChannel(channel);
            }
            else
            {
                _notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            }
            var notification = CreateProgressNotification(0, 100, true);

            StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeDataSync);
            return StartCommandResult.Sticky; // service restart if killed
        }
        
        private void Initialize()
        {
            string? mainDir = AppContext.BaseDirectory;
            var appDataDirectory = Path.Combine(FileSystem.AppDataDirectory, "MediaData");
            database = new DatabaseConnector(Path.Combine(mainDir, "portajeldb.sql"));
            var t = SaveHelper.LoadData(database, appDataDirectory);
            
            try
            {
                t.Wait();
                var result = t.Result; 
                serverConnector = (ServerConnector)result;
            }
            catch (Exception ex)
            {
                serverConnector = new();
                return;
            }

            // Add action to each loaded server and added server
            foreach (var server in serverConnector.Servers)
            {
                server.StartSyncActions.Add(StartSyncProgressAsync);
            }
            serverConnector.AddServerActions.Add((IMediaServerConnector server) =>
            {
                server.StartSyncActions.Add(StartSyncProgressAsync);
            });
        }

        private PendingIntent CreateBroadcastPendingIntent(string action)
        {
            var intent = new Intent(Platform.AppContext, typeof(NotificationActionReceiver));
            intent.SetAction(action);
            var flags = PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable;
            return PendingIntent.GetBroadcast(Platform.AppContext, action.GetHashCode(), intent, flags);
        }
        
        private Notification CreateProgressNotification(int progress, int max, bool indeterminate = false)
        {
            int hide = indeterminate ? NotificationCompat.PriorityMin : NotificationCompat.PriorityDefault;
            double percent = 0;
            if (!(progress == 0 && max == 0))
            {
                percent = progress * 100 / max;
            }
            Context context = Platform.AppContext;
            
            var tapPending = CreateBroadcastPendingIntent(NotificationActions.ACTION_NOTIFICATION_TAPPED);
            var deletePending = CreateBroadcastPendingIntent(NotificationActions.ACTION_NOTIFICATION_DELETED);
            
            return new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Syncing Data")
                .SetContentText($"Progress: {(int)percent}%")
                .SetSmallIcon(Resource.Drawable.abc_star_black_48dp)
                .SetContentIntent(tapPending)
                .SetDeleteIntent(deletePending)
                .SetPriority(hide)
                .SetVisibility(1)
                .SetSmallIcon(Resource.Drawable.abc_star_half_black_48dp)
                .SetColor(ContextCompat.GetColor(context, Resource.Color.primary_dark_material_dark))
                .SetOngoing(true)
                .SetProgress(max, progress, indeterminate)
                .Build();
        }
        
        public void UpdateNotification(int progress, int max)
        {
            var notification = CreateProgressNotification(progress, max);
            _notificationManager?.Notify(NOTIFICATION_ID, notification);
        }
        
        public async void StartSyncProgressAsync(CancellationToken cancellationToken)
        {
            if (isSyncRunning) return;
            isSyncRunning = true;
            while (isSyncRunning)
            {
                int total = 0;
                int count = 0;
                bool hasStarted = true;
                foreach (var item in Binder.Server.Servers)
                {
                    try
                    {
                        foreach (var data in item.DataConnectors)
                        {
                            if (data.Value != null)
                            {
                                total += data.Value.SyncStatusInfo.ServerItemTotal;
                                count += data.Value.SyncStatusInfo.ServerItemCount;
                                if(data.Value.SyncStatusInfo.TaskStatus == TaskStatus.WaitingToRun)
                                {
                                    hasStarted = false;
                                }
                            }
                            else
                            {
                                total += 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        total += 1;
                    }
                }
                UpdateNotification(count, total);
                await Task.Delay(500);
                if ((count >= total) && hasStarted)
                    break;
            }
            isSyncRunning = false;
            Context context = Platform.AppContext;
            var completedNotification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Sync Complete")
                .SetContentText("Data synchronization finished")
                .SetSmallIcon(Resource.Drawable.abc_star_black_48dp)
                .SetVisibility(1)
                .SetSmallIcon(Resource.Drawable.abc_star_half_black_48dp)
                .SetColor(ContextCompat.GetColor(context, Resource.Color.primary_dark_material_dark))
                .SetOngoing(false)
                .Build();
            _notificationManager?.Notify(NOTIFICATION_ID, completedNotification);
        }
    }
}
