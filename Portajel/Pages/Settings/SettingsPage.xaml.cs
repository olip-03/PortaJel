using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Structures;
using Portajel.Structures.ViewModels.Settings;

namespace Portajel.Pages.Settings;

public partial class SettingsPage : ContentPage
{
    private ServerConnector _serverConnector;
    private IDbConnector _dbConnector;
    
    private SettingsPageViewModel _viewModel = new();
    public SettingsPage(ServerConnector serverConnector, IDbConnector dbConnector)
	{
        _serverConnector = serverConnector;
        _dbConnector = dbConnector;
		InitializeComponent();
        _viewModel.ListItems = new()
        {
            new()
            {
                Title = "Connections",
                Description = "Manage connections to music service providers.",
                Icon = "hub.png",
                NavigationLocation = "settings/connections"
            },
            new()
            {
                Title = "Debug",
                Description = "Debugging options and interfaces for the backend.",
                Icon = "bug.png",
                NavigationLocation = "settings/debug"
            }
        };
        BindingContext = _viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ServerConnectionView = new(_serverConnector, _dbConnector);
        base.OnNavigatedTo(args);
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!e.CurrentSelection.Any()) return;
        if(e.CurrentSelection.First() is not ListItem listItem) return;
        if (sender is not CollectionView collectionView) return;
        await Shell.Current.GoToAsync(listItem.NavigationLocation);
        collectionView.SelectedItem = null;
    }
}