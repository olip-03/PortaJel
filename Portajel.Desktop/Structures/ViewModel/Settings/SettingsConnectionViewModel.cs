using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class SettingsConnectionViewModel : ReactiveObject
{
    private ServerConnector _server = Ioc.Default.GetService<ServerConnector>();
    private ObservableCollection<IMediaServerConnector> _connections = [];
    public ObservableCollection<IMediaServerConnector> Connections
    {
        get => _connections; 
        set => this.RaiseAndSetIfChanged(ref _connections, value);
    }
    public ReactiveCommand<Unit, Unit> NavigateToCommand { get; }
    public SettingsConnectionViewModel()
    {
        if (_server != null)
        {
            _connections.AddRange(_server.Servers);
        }    
    }
    public SettingsConnectionViewModel(ReactiveCommand<Unit, Unit> backCommand)
    {
        if (_server != null)
        {
            _connections.AddRange(_server.Servers);
        }
        NavigateToCommand = backCommand;
    }
}