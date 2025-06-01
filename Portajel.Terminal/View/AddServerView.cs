using Portajel.Connections.Services.Jellyfin;
using Portajel.Terminal.Struct;
using Portajel.Terminal.Struct.Interface;

namespace Portajel.Terminal.View;

public class AddServerView: IView
{
    public bool ShowTitle { get; set; } = true;
    public string[] Title { get; } = [
        @"  _____           _             _      _ ",
        @" |  __ \         | |           | |    | |",
        @" | |__) |__  _ __| |_ __ _     | | ___| |",
        @" |  ___/ _ \| '__| __/ _` |_   | |/ _ \ |",
        @" | |  | (_) | |  | || (_| | |__| |  __/ |",
        @" |_|   \___/|_|   \__\__,_|\____/ \___|_|",
        @" Add Server            Press Q to go back"
    ];    
    public string[] Contents { get; } = [];

    private bool _formSubmitted = false;
    public bool FormSubmitted
    {
        get
        {
            return _formSubmitted;
        }
        set
        {
            _formSubmitted = value;
            if (value == true)
            {
                AddAndStartAuth();
                Program.Back();
            }
        }
    }

    public List<FormItem> Form { get; } = new()
    {
        new FormItem("Server Address"),
        new FormItem("Username"),
        new FormItem("Password"),
    };
    public Dictionary<string, Action> Selections { get; } = new();
    public int Selected { get; set; }

    private void AddAndStartAuth()
    {
        Program.Server.AddServer(new JellyfinServerConnector(
            Program.Database,
            url: Form[0].UserResponse,
            username: Form[1].UserResponse,
            password: Form[2].UserResponse,
            appName: "PortaJel-Terminal",
            appVerison: "0.0.1",
            deviceName: Environment.MachineName, 
            deviceId: Environment.MachineName, 
            appDataPath: Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        ));
        Task.Run(async () =>
        {
            await Program.Server.AuthenticateAsync();
            await Program.Server.StartSyncAsync();
        });
    }
}