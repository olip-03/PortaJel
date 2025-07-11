using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Settings;

namespace Portajel.Pages.Settings;

public partial class HomeSettings : ContentPage
{
    private IServerConnector _server;
    private IDbConnector _database;

    private HomeSettingsViewModel _viewModel = new();
    
    public HomeSettings(IServerConnector server, IDbConnector database)
    {
        _server = server;
        _database = database;

        foreach (var srv in _server.Servers)
        {
            foreach (var feed in srv.Feeds.AvailableFeeds)
            {
                _viewModel.Feeds.Add(feed.Value);
            }
        }
        
        InitializeComponent();
        BindingContext = _viewModel;
    }
}