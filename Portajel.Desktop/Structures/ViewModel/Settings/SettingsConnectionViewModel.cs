using System.Collections.ObjectModel;
using System.Reactive;
using Portajel.Connections.Interfaces;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class SettingsConnectionViewModel : ReactiveObject
{
    private ObservableCollection<IMediaServerConnector> _connections = [];
    public ObservableCollection<IMediaServerConnector> Connections
    {
        get => _connections; 
        set => this.RaiseAndSetIfChanged(ref _connections, value);
    }

    public ReactiveCommand<Unit, Unit> NavigateToCommand { get; }
    
    public SettingsConnectionViewModel()
    {
        
    }
    public SettingsConnectionViewModel(ReactiveCommand<Unit, Unit> backCommand)
    {
        NavigateToCommand = backCommand;
    }
}