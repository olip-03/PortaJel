using System.Data;
using System.Reactive;
using Avalonia.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class MainWindowViewModel: ReactiveObject, IScreen
{
    private IDbConnector _database = Ioc.Default.GetService<IDbConnector>();

    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; } = new RoutingState();

    // The command that navigates a user to first view model.
    public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoAlbum { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoArtist { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoSong { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoGenre { get; }

    private SolidColorBrush _panelColor = SolidColorBrush.Parse("#31363B");
    public SolidColorBrush PanelColor
    {
        get => _panelColor;
        set => this.RaiseAndSetIfChanged(ref _panelColor, value);
    }
    private bool _settingsVisible = false;
    public bool SettingsVisible
    {
        get => _settingsVisible;
        set => this.RaiseAndSetIfChanged(ref _settingsVisible, value);
    }
    private string _testString = "Test";
    public string TestString
    {
        get => _testString;
        set => this.RaiseAndSetIfChanged(ref _testString, value);
    }
    
    public SettingsPanelViewModel SettingsPanel { get; }
    
    public MainWindowViewModel()
    {
        // Manage the routing state. Use the Router.Navigate.Execute
        // command to navigate to different view models. 
        //
        // Note, that the Navigate.Execute method accepts an instance 
        // of a view model, this allows you to pass parameters to 
        // your view models, or to reuse existing view models.
        //
        SettingsPanel = new SettingsPanelViewModel();
        GoHome = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new HomeViewModel(this))
        );
        GoAlbum = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new LibraryViewModel(this, _database.Connectors.Album))
        );
        GoArtist = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new LibraryViewModel(this, _database.Connectors.Artist))
        );
        GoSong = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new LibraryViewModel(this, _database.Connectors.Song))
        );
        GoGenre = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new LibraryViewModel(this, _database.Connectors.Genre))
        );
    }
}