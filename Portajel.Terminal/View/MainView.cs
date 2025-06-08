using Portajel.Terminal.Struct.Interface;
using Portajel.Connections;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Terminal.Struct;
using SkiaSharp;

namespace Portajel.Terminal.View
{
    public class MainView : IView
    {
        private ServerConnector _server;
        public MainView(ServerConnector server)
        {
            _server = server;
        }

        public bool ShowTitle { get; set; } = true;
        public string[] Title { get; } = [
            @"  _____           _             _      _ ",
            @" |  __ \         | |           | |    | |",
            @" | |__) |__  _ __| |_ __ _     | | ___| |",
            @" |  ___/ _ \| '__| __/ _` |_   | |/ _ \ |",
            @" | |  | (_) | |  | || (_| | |__| |  __/ |",
            @" |_|   \___/|_|   \__\__,_|\____/ \___|_|",
            @" Debugging Console        Press Q to quit"
        ];
        
        public string[] Contents
        {
            get
            {
                return srvStatus();
            }
        }
        public bool FormSubmitted { get; set; }
        public List<FormItem> Form { get; } = new();

        public Dictionary<string, Action?> Selections { get; } = new Dictionary<string, Action?>
        {
            { "Add Server", () => Program.SetView(new AddServerView()) },
            { "Remove Server", () => Program.SetView(new DeleteServerView()) },
            { "Authenticate All", DoAuth },
            { "", null},
            { "View Albums", () => Program.SetView(new LibraryView()) },
            { "View Artists", () => Program.SetView(new LibraryView()) },
            { "View Playlists", () => Program.SetView(new LibraryView()) },
            { "View Songs", () => Program.SetView(new LibraryView()) },
            { "View Genres", () => Program.SetView(new LibraryView()) },
        };

        public int Selected { get; set; } = 0;

        private static void DoAuth()
        {
            Task.Run(async () =>
            {
                await Program.Server.AuthenticateAsync();
                await Program.Server.StartSyncAsync();
            });
        }
        
        private string[] srvStatus()
        {
            List<string> status = new();

            if (_server.Servers.Any())
            {
                foreach (var srv in _server.Servers)
                {
                    switch (srv.AuthStatus.State)
                    {
                        case AuthState.NotStarted:
                            status.Add($"{srv.Name} - Waiting for sign-in");
                            break;
                        case AuthState.InProgress:
                            status.Add($"{srv.Name} - Signing in...");
                            break;
                        case AuthState.Success:
                            status.Add($"{srv.Name} - Logged in as {srv.GetUsername()}");
                            break;
                        case AuthState.Failed:
                            status.Add($"{srv.Name} - Failed to sign in!");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    status.Add("");
                    foreach (var data in srv.GetDataConnectors().Values)
                    {
                        if (data != null)
                        {
                            status.Add($"{data.MediaType.ToString()} {data.SyncStatusInfo.StatusPercentage}% - {data.SyncStatusInfo.ServerItemCount}/{data.SyncStatusInfo.ServerItemTotal}");
                        }
                    }
                }
            }
            else
            {
                status.Add("No Servers Added");
            }
                
            return TxtHelper.Box(status.ToArray());
        }
    }
}
