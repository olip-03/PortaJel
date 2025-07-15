using System.Diagnostics;
using Microsoft.Maui.Controls;
using Portajel.Components.FeedView;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages;

namespace Portajel.Pages
{
    public partial class HomePage : ContentPage
    {
        private IServerConnector _server;
        private IDbConnector _database;
        
        private readonly RefreshView _refreshView;
        private readonly ScrollView _scrollViewMain;
        private readonly Grid _content;

        public HomePage(IServerConnector server, IDbConnector dbConnector)
        {
            _server = server;
            _database = dbConnector;

            _content = new Grid();
            _scrollViewMain = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                Content = _content
            };
            _refreshView = new RefreshView
            {
                Content = _scrollViewMain,
                Command = new Command(() =>
                {
                    BuildUi();
                    _server.Feeds?.Refresh();
                    if (_refreshView != null) _refreshView.IsRefreshing = false;
                })
            };

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Title = "PortaJel";
            BindingContext = new HomePageViewModel();

            var settingsToolbarItem = new ToolbarItem
            {
                Text = "Settings",
                IconImageSource = "settings.png",
            };
            settingsToolbarItem.Clicked += NavigateSettings;
            ToolbarItems.Add(settingsToolbarItem);
            BuildUi();
        }
        
        private async Task<ConnectorFeeds> AwaitInit()
        {
            ConnectorFeeds toReturn = null;
            await Task.Run(async () =>
            {
                try
                {
                    int catchLimit = 0;
                    while (true)
                    {
                        try
                        {
                            if (_server.Feeds != null)
                            {
                                toReturn = _server.Feeds;
                                break;
                            }
                            else
                            {
                                await Task.Delay(1000);
                            }
                        }
                        catch (Exception e)
                        {
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write($"Loading Home Data Failed: {ex.Message}");
                }
            });
            _server.Feeds.Refresh();
            return toReturn;
        }

        private async void BuildUi()
        {
            var feedConnector = await AwaitInit();
            
            _content.Children.Clear();
            _content.RowDefinitions.Clear();

            for (int i = 0; i < feedConnector.Count; i++)
            {
                var feed = feedConnector.ElementAt(i);
                _content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                
                if (feed.Value.IsEnabled)
                {
                    HorizontalFeedView feedView = new(feed.Value);
                    Grid.SetRow(feedView, i);
                    _content.Children.Add(feedView);
                }
            }

            Content = _refreshView;
        }

        private async void NavigateSettings(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("settings");
        }
    }
}