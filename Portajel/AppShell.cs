using Microsoft.Maui.Controls;
using Portajel.Pages.Settings;
using Portajel.Pages.Settings.Connections;
using Portajel.Pages.Settings.Debug;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShellItem = Microsoft.Maui.Controls.ShellItem;
using Portajel.Pages.Views;
using Portajel.Components;
using Portajel.Structures.Functional;

namespace Portajel
{
    public partial class AppShell : Shell
    {
        private Color _primaryColor = Color.FromRgba(0, 0, 0, 255);

        public AppShell()
        {
            Title = "Portajel";

            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("settings/viewConnection", typeof(ViewConnectionPage));
            Routing.RegisterRoute("settings/home", typeof(HomeSettings));
            Routing.RegisterRoute("settings/debug", typeof(DebugPage));
            Routing.RegisterRoute("settings/debug/radio", typeof(DebugRadio));
            Routing.RegisterRoute("settings/debug/map", typeof(DebugMap));
            Routing.RegisterRoute("settings/debug/database", typeof(DebugDatabase));
            Routing.RegisterRoute("album", typeof(AlbumPage));

            Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += (s, a) =>
            {
                UpdateTheme();
            };
            UpdateTheme();

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                FlyoutBehavior = FlyoutBehavior.Locked;
                FlyoutBackdrop = _primaryColor;
                foreach (var item in DesktopTargetUI())
                {
                    Items.Add(item);
                }
            }
            else
            {
                FlyoutBehavior = FlyoutBehavior.Disabled;
                foreach (var item in MobileTargetUI())
                {
                    Items.Add(item);
                }
            }            
        }

        private void UpdateTheme()
        {
            var platformTheme = Microsoft.Maui.Controls.Application.Current.PlatformAppTheme;

            switch (platformTheme)
            {
                case AppTheme.Dark:
                    ThemeHelper.UpdatePrimaryColor("Dark");
                    break;
                default:
                    ThemeHelper.UpdatePrimaryColor("Light");
                    break;
            }
            
            var hasPrimaryColor = Microsoft.Maui.Controls.Application.Current.Resources.TryGetValue("Primary", out object primaryColor);
            if (hasPrimaryColor)
            {
                _primaryColor = (Color)primaryColor;
            }
        }
        
        private ShellItem[] DesktopTargetUI()
        {
            var homeItem = new FlyoutItem
            {
                Title = "Home",
                Icon = "home.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.HomePage))
                            }
                        }
                    }
                }
            };
            var playlistItem = new FlyoutItem
            {
                Title = "Playlists",
                Icon = "playlist.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.Library.PlaylistListPage))
                            }
                        }
                    }
                }
            };
            var albumItem = new FlyoutItem
            {
                Title = "Albums",
                Icon = "album.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.Library.AlbumListPage))
                            }
                        }
                    }
                }
            };
            var artistItems = new FlyoutItem
            {
                Title = "Artists",
                Icon = "artist.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.Library.ArtistListPage))
                            }
                        }
                    }
                }
            };
            var songsItems = new FlyoutItem
            {
                Title = "Songs",
                Icon = "song.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.Library.SongListPage))
                            }
                        }
                    }
                }
            };
            var genreItems = new FlyoutItem
            {
                Title = "Genres",
                Icon = "genre.png",
                Items =
                {
                    new Tab
                    {
                        Items =
                        {
                            new ShellContent
                            {
                                ContentTemplate = new DataTemplate(typeof(Pages.Library.GenreListPage))
                            }
                        }
                    }
                }
            };
            return [homeItem, playlistItem, albumItem, artistItems, songsItems, genreItems];
        }
        private ShellItem[] MobileTargetUI()
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
            
            return [tabBar];
        }
    }
}
