using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Portajel.Components;
using Portajel.Pages.Settings;
using Portajel.Pages.Settings.Connections;
using Portajel.Pages.Settings.Debug;
using Portajel.Pages.Views;
using Portajel.Structures.Functional;
using System.Diagnostics;
using ShellItem = Microsoft.Maui.Controls.ShellItem;

namespace Portajel
{
    public partial class AppShell : Shell
    {
        private Color _primaryColor = Color.FromRgba(0, 0, 0, 255);
        private Color _tertiaryBackground = Color.FromRgba(0, 0, 0, 255);

        private BottomNavBar _bottomNavBar = null!;
        private StatusBarBehavior? statusBar = null;

        public AppShell()
        {
            var app = Microsoft.Maui.Controls.Application.Current;
            if (app == null)
                throw new Exception("App cannot be null");

            Title = "Portajel";

            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("settings/viewConnection", typeof(ViewConnectionPage));
            Routing.RegisterRoute("settings/home", typeof(HomeSettings));
            Routing.RegisterRoute("settings/debug", typeof(DebugPage));
            Routing.RegisterRoute("settings/debug/radio", typeof(DebugRadio));
            Routing.RegisterRoute("settings/debug/map", typeof(DebugMap));
            Routing.RegisterRoute("settings/debug/database", typeof(DebugDatabase));
            Routing.RegisterRoute("album", typeof(AlbumPage));
            Routing.RegisterRoute("artist", typeof(ArtistPage));

            app.RequestedThemeChanged += (s, a) =>
            {
                UpdateTheme();
            };
            UpdateTheme();

            _bottomNavBar = BuildTabBar();
                
            FlyoutBehavior = FlyoutBehavior.Disabled;
            Items.Add(_bottomNavBar);
        }

        private void UpdateTheme()
        {
            var app = Microsoft.Maui.Controls.Application.Current;
            if (app == null)
                return;

            StatusBarStyle statusBarStyle = StatusBarStyle.Default;
            var platformTheme = Microsoft.Maui.Controls.Application.Current?.PlatformAppTheme;
            switch (platformTheme)
            {
                case AppTheme.Dark:
                    statusBarStyle = StatusBarStyle.LightContent;
                    ThemeHelper.UpdatePrimaryColor("Dark");
                    break;
                default:
                    ThemeHelper.UpdatePrimaryColor("Light");
                    statusBarStyle = StatusBarStyle.DarkContent;
                    break;
            }

            var hasPrimaryColor = app.Resources.TryGetValue("Primary", out object primaryColor);
            if (hasPrimaryColor == true)
            {
                _primaryColor = (Color)primaryColor;
            }
            var hasTertiaryBackground = app.Resources.TryGetValue("TertiaryBackground", out object tertiaryBackground);
            if (hasTertiaryBackground)
            {
                _tertiaryBackground = (Color)tertiaryBackground;
            }
            BackgroundColor = _tertiaryBackground;

            if (statusBar != null)
            {
                this.Behaviors.Remove(statusBar);
            }
            
            statusBar = new StatusBarBehavior
            {
                StatusBarColor = _tertiaryBackground,
                StatusBarStyle = statusBarStyle,
            };
            this.Behaviors.Add(statusBar);
        }

        private BottomNavBar BuildTabBar(Color? background = null)
        {
            var tabBar = new BottomNavBar();
            var homeTab = new Tab
            {
                Title = "Home",
                Icon = "home.png"
            };
            homeTab.Items.Add(new ShellContent
            {
                ContentTemplate = new DataTemplate(typeof(Pages.HomePage))
            });

            var searchTab = new Tab
            {
                Title = "Search",
                Icon = "search.png"
            };
            searchTab.Items.Add(new ShellContent
            {
                ContentTemplate = new DataTemplate(typeof(Pages.SearchPage))
            });

            var libraryTab = new Tab
            {
                Title = "Library",
                Icon = "library.png"
            };
            libraryTab.Items.Add(new ShellContent
            {
                Title = "Playlists",
                ContentTemplate = new DataTemplate(typeof(Pages.Library.PlaylistListPage))
            });
            libraryTab.Items.Add(new ShellContent
            {
                Title = "Albums",
                ContentTemplate = new DataTemplate(typeof(Pages.Library.AlbumListPage))
            });
            libraryTab.Items.Add(new ShellContent
            {
                Title = "Artists",
                ContentTemplate = new DataTemplate(typeof(Pages.Library.ArtistListPage))
            });
            libraryTab.Items.Add(new ShellContent
            {
                Title = "Songs",
                ContentTemplate = new DataTemplate(typeof(Pages.Library.SongListPage))
            });
            libraryTab.Items.Add(new ShellContent
            {
                Title = "Genres",
                ContentTemplate = new DataTemplate(typeof(Pages.Library.GenreListPage))
            });

            tabBar.Items.Add(homeTab);
            tabBar.Items.Add(searchTab);
            tabBar.Items.Add(libraryTab);
            
            return tabBar;
        }
    }
}
