using Android.App;
using Android.Runtime;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Portajel.Droid
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            ConfigureEdgeToEdgeHandlers();
        }
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        
        private static void ConfigureEdgeToEdgeHandlers()
        {

        }
    }
}
