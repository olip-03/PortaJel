using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;

namespace Portajel.Components;

public class MusicFeedView : Grid
{
    private IServerConnector _server;
    public MusicFeedView(IServerConnector server)
    {
        _server = server;
        BuildUI();
    }
    private void BuildUI()
    {
        Grid grid = new Grid();

        var mediaFeeds = _server.Feeds.AvailableFeeds.Values.Where(f => f.IsEnabled).ToArray();
        for (int i = 0; i < mediaFeeds.Length; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            var feed = mediaFeeds.ElementAt(i);
            
            ScrollView scrollView = new ScrollView();
            Grid layout = new Grid();
        }
    }
}