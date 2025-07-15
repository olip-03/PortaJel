using Portajel.Connections.Interfaces;

namespace Portajel.Connections;

public sealed class ServerConnectorFeeds: ConnectorFeeds
{
    private ServerConnector _serverConnector;
    public ServerConnectorFeeds(ServerConnector serverConnector)
    {
        _serverConnector= serverConnector;
        Refresh();
    }
    public override void Refresh()
    {
        Clear();
        foreach (var server in _serverConnector.Servers)
        {
            if(server.Feeds == null) continue;
            foreach (var feed in server.Feeds)
            {
                Add(feed.Key, feed.Value);
            }
        }
    }
}