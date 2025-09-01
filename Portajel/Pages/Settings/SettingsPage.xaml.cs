using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Structures;
using Portajel.Structures.ViewModels.Settings;

namespace Portajel.Pages.Settings;

public partial class SettingsPage : ContentPage
{
    private IServerConnector _serverConnector;
    private IDbConnector _dbConnector;
    
    private SettingsPageViewModel _viewModel = new();
    public SettingsPage(IServerConnector serverConnector, IDbConnector dbConnector)
	{
        _serverConnector = serverConnector;
        _dbConnector = dbConnector;
		InitializeComponent();
        _viewModel.ListItems = new()
        {
            new()
            {
                Title = "Home Page",
                Description = "Customise what's shown on the home page",
                Icon = "home.png",
                NavigationPage = new HomeSettings(serverConnector, dbConnector)
            },
            new()
            {
                Title = "Debug",
                Description = "Debugging options and interfaces for the backend.",
                Icon = "bug.png",
                NavigationPage = new DebugPage(serverConnector, dbConnector)
            }
        };
        BindingContext = _viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        ServerConnectionView = new(_serverConnector, _dbConnector);
        ServerConnectionView.RefreshConnections();
        base.OnNavigatedTo(args);
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!e.CurrentSelection.Any()) return;
        if(e.CurrentSelection.First() is not ListItem listItem) return;
        if (sender is not CollectionView collectionView) return;
        await Navigation.PushModalAsync(listItem.NavigationPage);
        collectionView.SelectedItem = null;
    }

    private async void FakeShellHeader_BackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}