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
    public ConnectorProperties Properties { get; set; }
    public FeedViewStyle ViewStyle { get; set; }
    private readonly IDbConnector? _database;
    
    public JellyfinRecentlyAdded(IDbConnector database, ConnectorProperties properties)
    {
        _database = database;
        Properties = properties;
        ServerUrl = Properties["ServerUrl"].Value.ToString();
        
        System.Enum.TryParse<FeedViewStyle>(Properties["ServerUrl"].Value.ToString(), out var parsed);
        ViewStyle = parsed;
    }
    public JellyfinRecentlyAdded(IDbConnector database, string serverUrl, bool isEnabled, FeedViewStyle viewStyle)
    {
        _database = database;
        ServerUrl = serverUrl;
        IsEnabled = isEnabled;
        ViewStyle = viewStyle;
        Properties = new ConnectorProperties()
        {
            {
                "ServerUrl", 
                new ConnectorPropertyValue
                {
                    Label = "Server Url",
                    Value = ServerUrl,
                    Icon = "hub.svg"
                }
            },
            {
                "ViewStyle", 
                new ConnectorPropertyValue
                {
                    Label = "View Style",
                    Value = ViewStyle,
                }
            },
        };
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