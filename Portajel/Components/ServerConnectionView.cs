using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Shapes;
using Portajel.Components.Modal;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Jellyfin;
using Color = Microsoft.Maui.Graphics.Color;
using System.Diagnostics;

namespace Portajel.Components;

public class ServerConnectionView : Grid
{
    internal static IServiceProvider? ServiceProvider { get; set; } = IPlatformApplication.Current?.Services;
    
    private Color _primaryDark = Color.FromRgba(0, 0, 0, 255);

    private IServerConnector _server = default!;
    private IDbConnector _database = default!;
    
    public ServerConnectionView()
    {
        // Its not DI but it works 
        _server = ServiceProvider.GetService<IServerConnector>();
        _database = ServiceProvider.GetService<IDbConnector>();
        BuildUI();
        
        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += (s, a) =>
        {
            BuildUI();
        };
    }
    
    public ServerConnectionView(IServerConnector server, IDbConnector dbConnector)
    {
        _server = server;
        _database = dbConnector;
        BuildUI();
        
        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += (s, a) =>
        {
            BuildUI();
        };
    }

    public void RefreshConnections()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        if (Application.Current is null) return;
        var imageColor = Application.Current.Resources.TryGetValue(
            "Primary", 
            out object primaryColor
        );
        if (imageColor)
        {
            _primaryDark = (Color)primaryColor;
        }
        
        Children.Clear();
        RowDefinitions.Clear();

        RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var connectionsGrid = GetConnectionsGrid(_server.Servers.ToArray());
        Grid.SetRow(connectionsGrid, 0);  // Use Grid.SetRow instead of SetRow
        Children.Add(connectionsGrid);

        var addButton = new Button
        {
            Margin = 10,
            Text = "Add Connection",
            Command = new Command(async () =>
            {
                string dataPath = FileSystem.AppDataDirectory;
                JellyfinServerConnector newServer = new JellyfinServerConnector(
                    _database, 
                    appDataPath: dataPath
                );
                await Application.Current.MainPage.Navigation.PushModalAsync(
                    new ModalAddServer(_server, newServer) 
                    { 
                        OnLoginSuccess = ((e) => { BuildUI(); }) 
                    }, 
                    true
                );
            })
        };
        Grid.SetRow(addButton, 1);  // Use Grid.SetRow instead of SetRow
        Children.Add(addButton);
    }

    private ScrollView GetConnectionsGrid(IMediaServerConnector[] connections)
    {
        var grid = new Grid();
        
        for (int i = 0; i < connections.Length; i++)
        {
            var serverConnection = connections[i];
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var button = new Button
            {
                Background = new SolidColorBrush(Color.FromRgba(0, 0, 0, 0)),
                Margin = 5,
                ZIndex = 5
            };
            button.Clicked += async (sender, e) => {
                await Shell.Current.GoToAsync(
                    "settings/viewConnection", 
                    new Dictionary<string, object>
                    {
                        { "Properties", serverConnection.Properties }
                    }
                );
            };
        
            var view = new VerticalStackLayout
            {
                ZIndex = 10,
                Children =
                {
                    new HorizontalStackLayout
                    {
                        new Image
                        {
                            Margin = 20,
                            Source = serverConnection.Image,
                            HeightRequest = 32,
                            WidthRequest = 32,
                            Behaviors =
                            {
                                new IconTintColorBehavior
                                {
                                    TintColor = _primaryDark
                                }
                            }
                        },
                        new VerticalStackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    Text = serverConnection.Name
                                },
                                new Label
                                {
                                    Text = serverConnection.Description,
                                    FontSize = 14,
                                    HorizontalOptions = LayoutOptions.Start
                                }
                            }
                        }
                    },
                    new ScrollView
                    {
                        CascadeInputTransparent = false,
                        Orientation = ScrollOrientation.Horizontal,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                        HeightRequest = 40,
                        Margin = new Thickness(10, 0, 10, 0),
                        Content = GetSyncChips(serverConnection)
                    }
                }
            };
            
            Grid.SetRow(button, i);
            Grid.SetRow(view, i);

            grid.Children.Add(button);
            grid.Children.Add(view);
        }

        return new ScrollView
        {
            Content = grid,
            VerticalOptions = LayoutOptions.FillAndExpand
        };
    }

    private Grid GetSyncChips(IMediaServerConnector serverConnection)
    {
        Grid mainGrid = new Grid();
        var items = serverConnection.DataConnectors;

        for (int i = 0; i < items.Count; i++)
        {
            var dataItem = items.ElementAt(i).Value;
            var dataName = items.ElementAt(i).Key;

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            Border main = new Border
            {
                Stroke = _primaryDark,
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(4, 2, 8, 2),
                Margin = new Thickness(0, 0, 8, 5),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 50
                },
                Content = new HorizontalStackLayout
                {
                    Children =
                    {
                        new Border
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            Background = _primaryDark,
                            Stroke = _primaryDark,
                            Margin = 4,
                            WidthRequest = 24,
                            HeightRequest = 24,
                            StrokeShape = new RoundRectangle
                            {
                                CornerRadius = 12
                            }
                        },
                        new Label
                        {
                            Text = dataName,
                            VerticalOptions = LayoutOptions.Center
                        }
                    }
                }
            };
            
            Grid.SetColumn(main, i);
            mainGrid.Children.Add(main);
        }

        return mainGrid;
    }
}