using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Jellyfin;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel.Settings;

public class AddConnectionViewModel: ReactiveObject
{
    private ServerConnector _server = Ioc.Default.GetService<ServerConnector>();
    private IDbConnector? _dbConnector = Ioc.Default.GetService<IDbConnector>();
    public ObservableCollection<string> SettingsCategories
    {
        get => _settingsCategories;
        set => this.RaiseAndSetIfChanged(ref _settingsCategories, value);
    }
    private ObservableCollection<string> _settingsCategories = ["Test", "check", "one", "two", "three", "four"];
    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }
    private bool _isBusy = false;
    public string ConnectionMessage
    {
        get => _connectionMessage;
        set => this.RaiseAndSetIfChanged(ref _connectionMessage, value);
    }
    private string _connectionMessage = "";
    public string ServerName
    {
        get => _serverName;
        set => this.RaiseAndSetIfChanged(ref _serverName, value);
    }
    private string _serverName = "";
    public string ServerUrl
    {
        get => _serverUrl;
        set => this.RaiseAndSetIfChanged(ref _serverUrl, value);
    }
    private string _serverUrl = "";
    public string ServerUsername
    {
        get => _serverUsername;
        set => this.RaiseAndSetIfChanged(ref _serverUsername, value);
    }
    private string _serverUsername = "";
    public string ServerPassword
    {
        get => _serverPassword;
        set => this.RaiseAndSetIfChanged(ref _serverPassword, value);
    }
    private string _serverPassword = "";
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    
    public AddConnectionViewModel()
    {
        
    }
    public AddConnectionViewModel(ReactiveCommand<Unit, Unit> backCommand)
    {
        NavigateBackCommand = backCommand;
        ConnectCommand = ReactiveCommand.Create(TryConnect);
    }

    private async void TryConnect()
    {
        try
        {
            IsBusy = true;
            var server = new JellyfinServerConnector(
                _dbConnector, 
                _serverUrl, 
                _serverUsername, 
                _serverPassword, 
                _serverName,
                "0.1",
                Environment.MachineName,
                Environment.UserName,
                Program.AppDataPath);
            var result = await server.AuthenticateAsync();
            if (result.State == AuthState.Success)
            {
                _server.AddServer(server);
                NavigateBackCommand.Execute();
            }
        }
        catch (Exception e)
        {
            ConnectionMessage = e.Message;
            Trace.WriteLine(e);
        }
        finally
        {
            IsBusy = false;
        }
    }
}