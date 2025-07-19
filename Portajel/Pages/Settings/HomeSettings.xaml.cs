using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Components.Modal;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using Portajel.Structures.Functional;
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
        UpdateList();
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        UpdateList();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        SaveHelper.SaveData(_server);
        base.OnNavigatingFrom(args);
    }

    private void UpdateList()
    {
        _viewModel.Feeds.Clear();
        foreach (var srv in _server.Servers)
        {
            foreach (var feed in srv.Feeds)
            {
                _viewModel.Feeds.Add(feed.Value);
            }
        }
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        string dataPath = FileSystem.AppDataDirectory;
        BaseMediaFeed feed = new BaseMediaFeed();
        await Application.Current.MainPage.Navigation.PushModalAsync(
            new ModalAddFeed(_server, feed) 
            { 
                OnLoginSuccess = ((e) => { UpdateList(); }) 
            }, 
            true
        );
    }
}