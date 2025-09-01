using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.View;
using FFImageLoading;
using FFImageLoading.Config;
using FFImageLoading.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Portajel.Render;
using System.Net.Http.Headers;
using Xamarin.Android.Net;
using Configuration = FFImageLoading.Config.Configuration;

namespace Portajel.Droid
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            var activity = this;
            if (activity?.Window != null)
            {
                // This is the core call to enable edge-to-edge
                WindowCompat.SetDecorFitsSystemWindows(Window ?? throw new Exception("Null check failed"), false);
                
                Window.SetFlags(WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
                
                var windowInsetsController = WindowCompat.GetInsetsController(activity.Window, activity.Window.DecorView);
                
                var decorView = Window?.DecorView;
                if (decorView != null)
                {
                    ViewCompat.SetOnApplyWindowInsetsListener(decorView, new CustomInsetsListener());
                }

                var imageService = ServiceHelper.GetService<IImageService>();
                var handler = new AndroidMessageHandler();
                var httpClient = new HttpClient(handler);
                imageService.Initialize(new Configuration
                {
                    HttpClient = httpClient
                });
            }
            base.OnCreate(savedInstanceState);
        }

        public override ActionMode? OnWindowStartingActionMode(ActionMode.ICallback? callback)
        {
            return base.OnWindowStartingActionMode(callback);
        }

        public class CustomInsetsListener : Java.Lang.Object, AndroidX.Core.View.IOnApplyWindowInsetsListener
        {
            public WindowInsetsCompat OnApplyWindowInsets(Android.Views.View v, WindowInsetsCompat insets)
            {
                // For modals, we want to draw behind the status bar
                // So we don't apply system bar insets
                return WindowInsetsCompat.Consumed;
            }
        }
    }
}
