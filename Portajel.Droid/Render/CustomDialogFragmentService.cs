using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using CommunityToolkit.Maui.Core;

namespace Portajel.Render
{

    public sealed class CustomDialogFragmentService : IDialogFragmentService
    {
        public void OnFragmentCreated(FragmentManager fm, Fragment f, Bundle? savedInstanceState)
        {
            // Implementation not needed for this use case
        }

        public void OnFragmentStarted(FragmentManager fm, Fragment f)
        {
            if (f is not DialogFragment dialogFragment)
                return;

            if (dialogFragment?.Dialog?.Window is not { } window)
                return;

            var background = BackgroundHelper.GetColor("BackgroundColor");
            // Apply your edge-to-edge settings to the modal dialog
            window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.SetNavigationBarColor(background.Color);
            
            //window.SetStatusBarColor(Android.Graphics.Color.Transparent);

            // For API 30+
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                window.SetDecorFitsSystemWindows(false);
            }

            // Apply edge-to-edge insets handling
            ApplyEdgeToEdgeInsets(window);
        }

        private void ApplyEdgeToEdgeInsets(Android.Views.Window window)
        {
            var decorView = window.DecorView;
            if (decorView != null)
            {
                ViewCompat.SetOnApplyWindowInsetsListener(decorView, new EdgeToEdgeInsetsListener());
            }
        }

        // Other required interface methods...
        public void OnFragmentResumed(FragmentManager fm, Fragment f) { }
        public void OnFragmentPaused(FragmentManager fm, Fragment f) { }
        public void OnFragmentStopped(FragmentManager fm, Fragment f) { }
        public void OnFragmentDestroyed(FragmentManager fm, Fragment f) { }
        public void OnFragmentSaveInstanceState(FragmentManager fm, Fragment f, Bundle savedInstanceState) { }
        public void OnFragmentViewCreated(FragmentManager fm, Fragment f, Android.Views.View v, Bundle? savedInstanceState) { }
        public void OnFragmentViewDestroyed(FragmentManager fm, Fragment f) { }
        public void OnFragmentActivityCreated(FragmentManager fm, Fragment f, Bundle? savedInstanceState) { }
        public void OnFragmentAttached(FragmentManager fm, Fragment f, Context context) { }
        public void OnFragmentDetached(FragmentManager fm, Fragment f) { }
        public void OnFragmentPreAttached(FragmentManager fm, Fragment f, Context context) { }
        public void OnFragmentPreCreated(FragmentManager fm, Fragment f, Bundle? savedInstanceState) { }
    }

    public class EdgeToEdgeInsetsListener : Java.Lang.Object, AndroidX.Core.View.IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat OnApplyWindowInsets(Android.Views.View v, WindowInsetsCompat insets)
        {
            // Don't apply any padding - let content draw behind status bar
            return insets;
        }
    }
}
