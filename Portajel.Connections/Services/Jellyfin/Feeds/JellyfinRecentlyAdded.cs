using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinRecentlyAdded(IDbConnector database, string serverUrl, bool isEnabled, FeedViewStyle viewStyle) : IMediaFeed
{
    public string Id { get; }= "JellyfinRecentlyAdded";
    public string Name { get; } = "Recently Added";

    public string Description => $"Most recently added music from {ServerUrl}.";
    public string ServerUrl { get; } = null!;
    public bool IsEnabled { get; set; }
    public List<ConnectorProperty> Properties { get; set; }
    public FeedViewStyle ViewStyle { get; } = viewStyle;
    private readonly IDbConnector? _database;
    
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        throw new NotImplementedException();
    }

    public int Total()
    {
        throw new NotImplementedException();
    }
}