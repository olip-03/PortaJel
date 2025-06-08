using Portajel.Connections.Interfaces;
using Portajel.Terminal.Struct;
using Portajel.Terminal.Struct.Interface;

namespace Portajel.Terminal.View;

public class DeleteServerView: IView
{
    public bool ShowTitle { get; set; } = true;
    public string[] Title { get; } = [
        @"  _____           _             _      _ ",
        @" |  __ \         | |           | |    | |",
        @" | |__) |__  _ __| |_ __ _     | | ___| |",
        @" |  ___/ _ \| '__| __/ _` |_   | |/ _ \ |",
        @" | |  | (_) | |  | || (_| | |__| |  __/ |",
        @" |_|   \___/|_|   \__\__,_|\____/ \___|_|",
        @" Delete Server         Press Q to go back"
    ];
    public string[] Contents { get; } = [];
    public bool FormSubmitted { get; set; } = false;
    public List<FormItem> Form { get; } = [];
    public Dictionary<string, Action?> Selections
    {
        get
        {
            Dictionary<string, Action> toReturn = new ();
            foreach (var item in Program.Server.Servers)
            {
                toReturn.Add(item.Name, () => RemoveServer(item));
            }

            return toReturn;
        }
    }
    public int Selected { get; set; } = 0;
    private void RemoveServer(IMediaServerConnector srvAddr)
    {
        Program.Server.RemoveServer(srvAddr);
    }
}