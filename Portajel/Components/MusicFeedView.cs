using Portajel.Connections.Interfaces;

namespace Portajel.Components;

public class MusicFeedView : ContentView
{
    private IServerConnector _server;
    public MusicFeedView(IServerConnector server)
    {
        _server = server;
        Content = Layout();
    }
    private Grid Layout()
    {
        Grid grid = new Grid();
        return grid;
    }
}