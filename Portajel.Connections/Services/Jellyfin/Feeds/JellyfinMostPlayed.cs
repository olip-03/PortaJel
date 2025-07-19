using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinMostPlayed : IMediaFeed
{
    private readonly IDbConnector? _database;
    public string Id { get; set; } = "JellyfinMostPlayed";
    public string Name { get; set; } = "Most Played";
    public string Description { get; set; } = "Most played from this library.";
    public string ServerUrl { get; set; }
    public bool IsEnabled { get; set; }
    public ConnectorProperties Properties { get; set; }
    public FeedViewStyle ViewStyle { get; set; }

    public JellyfinMostPlayed(IDbConnector database, ConnectorProperties properties)
    {
        _database = database;
        Properties = properties;
        ServerUrl = Properties["ServerUrl"].Value.ToString();
        
        System.Enum.TryParse<FeedViewStyle>(Properties["ServerUrl"].Value.ToString(), out var parsed);
        ViewStyle = parsed;
    }
    public JellyfinMostPlayed(IDbConnector database, string serverUrl, bool isEnabled, FeedViewStyle viewStyle)
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
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return _database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.PlayCount);
    }
    public int Total()
    {
        return _database.Connectors.Album.GetTotalCount();
    }
}