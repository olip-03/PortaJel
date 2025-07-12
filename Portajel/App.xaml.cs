using FFImageLoading;
using FFImageLoading.Config;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Functional;
using System.Diagnostics;
using Microsoft.Maui.Platform;

namespace Portajel
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; set; }
        public App(IServerConnector serverConnector, IDbConnector dbConnector, IServiceProvider services)
        {
            Services = services;
            InitializeComponent();
            string mainDir = FileSystem.Current.AppDataDirectory;

            // Fire & forget
            Sharpnado.Tabs.Initializer.Initialize(false, false);
            StartupAsync(serverConnector, dbConnector);
        }

        private void StartupAsync(IServerConnector serverConnector, IDbConnector dbConnector)
        {
            //IConfiguration imgConfig = new Configuration();
            //imgConfig.DecodingMaxParallelTasks = 4; 
            //imgConfig.VerboseLogging = false;
            //imgConfig.VerbosePerformanceLogging = false;
            //imgConfig.HttpHeadersTimeout = 15; 
            //imgConfig.HttpReadTimeout = 15;
            IImageService imgSvc = FFImageLoading.ImageService.Instance;

            if (OperatingSystem.IsAndroid())
            {
                // Sync & auth should occur after binder has connected. Code for 
                // auth is in the DroidService class.
                return;
            }
            _ = Task.Run(async () =>
            {
                if (serverConnector is not ServerConnector connector) return;
                await connector.AuthenticateAsync();
                await connector.StartSyncAsync();
                await SaveHelper.SaveData(connector);
            });
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new Window(new AppShell());
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                window.TitleBar = new TitleBar
                {
                    HeightRequest = 48,
                    Icon = "titlebar_icon.png",
                    Title = "PortaJel",
                    Subtitle = "Demo",
                    Content = new SearchBar
                    {
                        PlaceholderColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 300,
                        HeightRequest = 32,
                        Placeholder = "Search"
                    },
                    TrailingContent = new ImageButton
                    {
                        HeightRequest = 36,
                        WidthRequest = 36,
                        Source = new FontImageSource
                        { 
                            Size = 16,
                            Glyph = "&#xE713;",
                            FontFamily = "SegoeMDL2"
                        }
                    }
                };
            }

            return window;
        }
    }
}
