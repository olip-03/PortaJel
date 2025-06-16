using System.Reactive;
using Avalonia.Media;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class MainWindowViewModel: ReactiveObject, IScreen
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; } = new RoutingState();

    // The command that navigates a user to first view model.
    public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }

    // The command that navigates a user back.
    public ReactiveCommand<Unit, IRoutableViewModel> GoLibrary { get; }
    
    
    private SolidColorBrush _panelColor = SolidColorBrush.Parse("#31363B");
    public SolidColorBrush PanelColor
    {
        get => _panelColor;
        set => this.RaiseAndSetIfChanged(ref _panelColor, value);
    }
    
    private string _testString = "Test";
    public string TestString
    {
        get => _testString;
        set => this.RaiseAndSetIfChanged(ref _testString, value);
    }
    
    public MainWindowViewModel()
    {
        // Manage the routing state. Use the Router.Navigate.Execute
        // command to navigate to different view models. 
        //
        // Note, that the Navigate.Execute method accepts an instance 
        // of a view model, this allows you to pass parameters to 
        // your view models, or to reuse existing view models.
        //
        GoHome = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new HomeViewModel(this))
        );
        GoLibrary= ReactiveCommand.CreateFromObservable(
            () => Router.NavigateAndReset.Execute(new LibraryViewModel(this))
        );
    }
}