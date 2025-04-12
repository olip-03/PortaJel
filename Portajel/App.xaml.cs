using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Functional;
using System.Diagnostics;

namespace Portajel
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; set; }
        public App(IServerConnector serverConnector, IDbConnector dbConnector, IServiceProvider services)
        {
            Services = services;
            InitializeComponent();

            // Fire & forget
            _ = Task.Run(async () =>
            {
                try
                {
                    await StartupAsync(serverConnector, dbConnector);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Startup error: {ex.Message}");
                }
            });
        }

        private async Task StartupAsync(IServerConnector serverConnector, IDbConnector dbConnector)
        {
            if (OperatingSystem.IsAndroid())
            {
                // Sync & auth should occur after binder has connected. Code for 
                // auth is in the DroidService class.
                return;
            }
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));

            var auth = await serverConnector.AuthenticateAsync();
            var t = serverConnector.Servers.Select(s => s.UpdateDb());
            try
            {
                await Task.WhenAll(t);
                await SaveHelper.SaveData(serverConnector);
            }
            catch (Exception)
            {
                throw;
            };
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
