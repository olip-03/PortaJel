using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

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
                WindowCompat.SetDecorFitsSystemWindows(Window, false);
                Window.SetFlags(WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                activity.Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
            }
            base.OnCreate(savedInstanceState);
        }
        public override ActionMode? OnWindowStartingActionMode(ActionMode.ICallback? callback)
        {
            return base.OnWindowStartingActionMode(callback);
        }
    }
}
