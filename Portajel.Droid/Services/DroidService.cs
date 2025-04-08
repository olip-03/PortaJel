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

namespace Portajel.Droid.Services
{
    [Service(Name = "PortaJel.Droid.ServiceController", IsolatedProcess = true, ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
    public class DroidService : Service
    {
        // Add these fields to your class
        private NotificationManager? _notificationManager;
        private const int NOTIFICATION_ID = 517;
        private const string CHANNEL_ID = "sync_channel";
        public DroidServiceBinder Binder { get; set; } = default!;
        public DatabaseConnector database { get; private set; } = null!;
        public ServerConnector serverConnector { get; private set; } = null!;
        public DroidService()
        {
            
        }
        public override IBinder? OnBind(Intent? intent)
        {
            string? mainDir = System.AppContext.BaseDirectory;
            var appDataDirectory = Path.Combine(FileSystem.AppDataDirectory, "MediaData");
            var t = SaveHelper.LoadData(database, appDataDirectory);
            t.Wait();

            database = new DatabaseConnector(Path.Combine(mainDir, "portajeldb.sql"));
            serverConnector = (ServerConnector)t.Result;

            serverConnector.AddServerActions.Add((IMediaServerConnector server) =>
            {
                server.StartSyncActions.Add(StartSyncProgressAsync);
            });

            var srvAuth = serverConnector.AuthenticateAsync();
            var srvUpdate = Task.WhenAll(serverConnector.Servers.Select(s => s.UpdateDb()));
            try
            {
                srvAuth.Wait();
                srvUpdate.Wait();
            }
            catch (Exception)
            {
                throw;
            };

            Binder = new DroidServiceBinder(this);
            return Binder;
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
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
            var notification = CreateProgressNotification(0, 100);
            if (intent != null)
            {
                string? value = intent.GetStringExtra("APICredentials");
                if (value != null)
                {
                    var appDataDirectory = Path.Combine(FileSystem.AppDataDirectory, "MediaData");
                    ServerConnectorSettings settings = new(value, database, appDataDirectory);
                    serverConnector = (ServerConnector)settings.ServerConnector;

                    serverConnector.AddServerActions.Add((IMediaServerConnector server) =>
                    {
                        server.StartSyncActions.Add(StartSyncProgressAsync);
                    });
                }
            }

            StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeDataSync);
            return StartCommandResult.Sticky; // service restart if killed
        }
        private Notification CreateProgressNotification(int progress, int max)
        {
            double percent = progress * 100 / max;
            Context context = Platform.AppContext;
            return new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Syncing Data")
                .SetContentText($"Progress: {(int)percent}%")
                .SetSmallIcon(Resource.Drawable.abc_star_black_48dp)
                .SetPriority(NotificationCompat.PriorityLow)
                .SetVisibility(1)
                .SetSmallIcon(Resource.Drawable.abc_star_half_black_48dp)
                .SetColor(ContextCompat.GetColor(context, Resource.Color.primary_dark_material_dark))
                .SetOngoing(true)
                .SetProgress(max, progress, false)
                .Build();
        }
        public void UpdateNotification(int progress, int max)
        {
            var notification = CreateProgressNotification(progress, max);
            _notificationManager?.Notify(NOTIFICATION_ID, notification);
        }
        public void StartSyncProgressAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                int total = 0;
                int count = 0;
                foreach (var item in Binder.Server.Servers)
                {
                    try
                    {
                        foreach (var data in item.GetDataConnectors())
                        {
                            if (data.Value != null)
                            {
                                total += data.Value.SyncStatusInfo.ServerItemTotal;
                                count += data.Value.SyncStatusInfo.ServerItemCount;
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
                if (count >= total)
                    break;
            }
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
