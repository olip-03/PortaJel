﻿using Microsoft.Maui.Controls;
using Portajel.Pages.Settings;
using Portajel.Pages.Settings.Connections;
using Portajel.Pages.Settings.Debug;
using PortaJel_Blazor.Classes;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ShellItem = Microsoft.Maui.Controls.ShellItem;

namespace Portajel
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Title = "Portajel";

            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("settings/connections", typeof(ConnectionsPage));
            Routing.RegisterRoute("settings/connections/view", typeof(ViewConnectionPage));
            Routing.RegisterRoute("settings/debug", typeof(DebugPage));
            Routing.RegisterRoute("settings/debug/radio", typeof(DebugRadio));
            Routing.RegisterRoute("settings/debug/map", typeof(DebugMap));
            Routing.RegisterRoute("settings/debug/database", typeof(DebugDatabase));
            
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                FlyoutBehavior = FlyoutBehavior.Locked;
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
            var tabBar = new TabBar();
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
