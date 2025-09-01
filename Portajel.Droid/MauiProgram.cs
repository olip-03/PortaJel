using Android.Net.Http;    // For AndroidMessageHandler (newer)
using FFImageLoading;
using FFImageLoading.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Portajel;
using Portajel.Components;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Droid.Playback;
using Portajel.Droid.Render;
using Portajel.Droid.Services;
using Portajel.Pages.Settings;
using Portajel.Pages.Settings.Debug;
using Portajel.Render;
using Portajel.Services;
using Portajel.Structures.Interfaces;
using PortaJel.Droid.Services;
using System.Net.Http.Headers;
using Xamarin.Android.Net;

/// Reference for Android Tracing 
/// https://github.com/dotnet/android/blob/main/Documentation/guides/tracing.md
/// 

namespace Portajel.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string? mainDir = AppContext.BaseDirectory;
            if (mainDir == null) throw new SystemException("Could not find the main directory of the application.");
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddSingleton<DroidServiceController>();

            builder.Services.AddSingleton<IDbConnector, DroidDbConnector>(serviceProvider => {
                var service = serviceProvider.GetRequiredService<DroidServiceController>();
                DroidDbConnector droidServer = new DroidDbConnector(service);
                return droidServer;
            });

            builder.Services.AddSingleton<IServerConnector, DroidServerConnector>(serviceProvider => {
                var service = serviceProvider.GetRequiredService<DroidServiceController>();
                DroidServerConnector droidServer = new DroidServerConnector(service);
                return droidServer;
            });

            // Enables modal edge-to-edge
            builder.ConfigureLifecycleEvents(lifecycleBuilder =>
            {
                lifecycleBuilder.AddAndroid(androidBuilder =>
                {
                    androidBuilder.OnCreate((activity, _) =>
                    {
                        if (activity is AndroidX.AppCompat.App.AppCompatActivity appCompatActivity)
                        {
                            var fragmentManager = appCompatActivity.SupportFragmentManager;
                            fragmentManager?.RegisterFragmentLifecycleCallbacks(
                                new CommunityToolkit.Maui.Core.Services.FragmentLifecycleManager(
                                    new CustomDialogFragmentService()),
                                false);
                        }
                    });
                });
            });

            builder.Services.AddSingleton<IMediaController, MediaController>();
            builder.Services.AddSingleton<DroidServiceBinder>();
            builder.UseSharedMauiApp().ConfigureMauiHandlers((handlers) =>
            {
                handlers.AddHandler(typeof(AppShell), typeof(NavShellRenderer));
            });

            return builder.Build();
        }
    }
}
