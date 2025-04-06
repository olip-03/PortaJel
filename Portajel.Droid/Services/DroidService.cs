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

            return new DroidServiceBinder(this);
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Setup notification channel
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

            // Create initial notification with progress bar
            var notification = CreateProgressNotification(0, 100);

            // Process intent
            if (intent != null)
            {
                string? value = intent.GetStringExtra("APICredentials");
                if (value != null)
                {
                    var appDataDirectory = Path.Combine(FileSystem.AppDataDirectory, "MediaData");
                    ServerConnectorSettings settings = new(value, database, appDataDirectory);
                    serverConnector = (ServerConnector)settings.ServerConnector;

                    // Start a background task to simulate/track progress
                    _ = Task.Run(async () => await UpdateSyncProgressAsync());
                }
            }

            // Start foreground with notification
            StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeDataSync);

            // Return Sticky to have service restart if killed
            return StartCommandResult.Sticky;
        }

        // Helper method to create/update progress notification
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
                .SetProgress(max, progress, false) // false = determinate progress bar
                .Build();
        }

        // Method to update progress notification
        public void UpdateNotification(int progress, int max)
        {
            var notification = CreateProgressNotification(progress, max);
            _notificationManager?.Notify(NOTIFICATION_ID, notification);
        }

        // Example method to simulate progress (replace with your actual progress tracking)
        private async Task UpdateSyncProgressAsync()
        {
            await Task.Delay(4000);
            while (true)
            {
                // Init
                int total = 0;
                int count = 0;
                foreach (var item in Binder.Server.Servers)
                {
                    foreach (var data in item.GetDataConnectors())
                    {
                        if(data.Value != null)
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
                UpdateNotification(count, total);
                await Task.Delay(100);
                if (count >= total)
                    break;
            }

            // When complete, update notification
            Context context = Platform.AppContext;
            var completedNotification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Sync Complete")
                .SetContentText("Data synchronization finished")
                .SetSmallIcon(Resource.Drawable.abc_star_black_48dp)
                .SetVisibility(1)
                .SetSmallIcon(Resource.Drawable.abc_star_half_black_48dp)
                .SetColor(ContextCompat.GetColor(context, Resource.Color.primary_dark_material_dark))
                .SetOngoing(false) // Allow dismissing when complete
                .Build();

            _notificationManager?.Notify(NOTIFICATION_ID, completedNotification);
        }
    }
}
