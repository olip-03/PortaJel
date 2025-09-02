using Portajel.Connections.Interfaces;
using Portajel.Pages.Settings.Debug;
using Portajel.Structures;
using Portajel.Structures.ViewModels.Settings;

namespace Portajel.Pages.Settings;

public partial class DebugPage : ContentPage
{
    private SettingsPageViewModel _viewModel = new();
    public DebugPage(IServerConnector serverConnector, IDbConnector dbConnector)
	{
		InitializeComponent();
        _viewModel.ListItems = new()
        {
            new()
            {
                Title = "Debug Radio",
                Description = "Backend radio options for testing features.",
                Icon = "radio.png",
                NavigationPage = new DebugRadio()
            },
            new()
            {
                Title = "Debug Map",
                Description = "Test and configure map features.",
                Icon = "map.png",
                NavigationPage = new DebugMap()
            },
            new()
            {
                Title = "Debug Database",
                Description = "Test and configure the database.",
                Icon = "database.png",
                NavigationPage =new DebugDatabase(serverConnector, dbConnector)
            }
        };
        BindingContext = _viewModel;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!e.CurrentSelection.Any()) return;
        if (e.CurrentSelection.First() is not ListItem listItem) return;
        if (sender is not CollectionView collectionView) return;
        collectionView.SelectedItem = null;
        await Navigation.PushModalAsync(listItem.NavigationPage);
    }

    private async void FakeShellHeader_BackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}