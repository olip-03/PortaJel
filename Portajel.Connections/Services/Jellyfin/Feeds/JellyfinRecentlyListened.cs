using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinRecentlyListened: IMediaFeed
{
    public string Id { get; set;  } = "JellyfinRecentlyListened";
    public string Name { get; set; } = "Recently Listened";
    public string Description { get; set; } = "Music you've recently played";
    public string ServerUrl { get; set;  }
    public bool IsEnabled { get; set; }
    public List<ConnectorPropertyValue> Properties { get; set; }
    public FeedViewStyle ViewStyle { get; }
    private readonly IDbConnector? _database;

    public JellyfinRecentlyListened()
    {
        
    }
    public JellyfinRecentlyListened(IDbConnector database)
    {
        _database = database;
    }
    public JellyfinRecentlyListened(IDbConnector database, string serverUrl, bool isEnabled, FeedViewStyle viewStyle)
    {
        ViewStyle = viewStyle;
        _database = database;
    }
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return _database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.PlayCount);
    }
    public int Total()
    {
        return _database.Connectors.Album.GetTotalCount();
    }
}