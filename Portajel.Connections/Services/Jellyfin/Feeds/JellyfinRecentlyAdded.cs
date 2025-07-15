using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinRecentlyAdded : IMediaFeed
{
    public string Id { get; set; }= "JellyfinRecentlyAdded";
    public string Name { get; set; } = "Recently Added";

    public string Description
    {
        get { return $"Most recently added music from {ServerUrl}."; }
        set { }
    }
    public string ServerUrl { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public List<ConnectorPropertyValue> Properties { get; set; }
    public FeedViewStyle ViewStyle { get; }
    private readonly IDbConnector? _database;

    public JellyfinRecentlyAdded(IDbConnector database, string serverUrl, bool isEnabled, FeedViewStyle viewStyle)
    {
        ViewStyle = viewStyle;
        _database = database;
    }
    public JellyfinRecentlyAdded(IDbConnector database)
    {
        _database = database;
    }
    public JellyfinRecentlyAdded()
    {
        
    }
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return _database == null? [] : _database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.DateCreated);
    }

    public int Total()
    {
        return _database == null ? 0 : _database.Connectors.Album.GetTotalCount();
    }
}