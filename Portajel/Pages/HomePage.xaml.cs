using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Portajel.Components.FeedView;
using Portajel.Connections.Interfaces;
using Portajel.Pages.Settings;
using Portajel.Structures.ViewModels.Pages;
using Portajel.Connections;
namespace Portajel.Pages
{
    public partial class HomePage : ContentPage
    {
        readonly ServerConnector _server;
        readonly IDbConnector _database;

        public HomePage(ServerConnector server, IDbConnector database)
        {
            InitializeComponent();

            _server = server;
            _database = database;

            // set up your VM (if it exposes RefreshCommand / IsRefreshing)
            BindingContext = new HomePageViewModel(FeedList);

            // if you prefer hooking up pull-to-refresh here instead:
            RefreshViewMain.Command = new Command(async () =>
            {
                _server.Feeds?.Refresh();
                RefreshViewMain.IsRefreshing = false;
            });

            // initial build
        }

        public async void NavigateSettings(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
                new SettingsPage(_server, _database));
        }
    }
}