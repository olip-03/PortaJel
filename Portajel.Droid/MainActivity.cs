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
                var test = GetSafeAreaInsets();

                // This is the core call to enable edge-to-edge
                WindowCompat.SetDecorFitsSystemWindows(Window, false);
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // Optional: Make status and navigation bars transparent
                // You might need to adjust colors based on your app's theme
                activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                activity.Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
            }


            base.OnCreate(savedInstanceState);
        }

        public Thickness GetSafeAreaInsets()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
                return new Thickness(0);

            var activity = this;
            var insets = AndroidX.Core.View.ViewCompat.GetRootWindowInsets(activity.Window.DecorView)
                ?.GetInsets(AndroidX.Core.View.WindowInsetsCompat.Type.SystemBars());

            Resources resources = ApplicationContext.Resources;
            int resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                var size = resources.GetDimensionPixelSize(resourceId);
            }

            if (insets == null)
                return new Thickness(0);

            return new Thickness(
                insets.Left,
                insets.Top,
                insets.Right,
                insets.Bottom);
        }


        public override ActionMode? OnWindowStartingActionMode(ActionMode.ICallback? callback)
        {

            return base.OnWindowStartingActionMode(callback);
        }
    }
}
