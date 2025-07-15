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
    public List<ConnectorPropertyValue> Properties { get; set; }
    public JellyfinMostPlayed(IDbConnector database, string serverUrl, bool isEnabled)
    {
        _database = database;
        ServerUrl = serverUrl;
        IsEnabled = isEnabled;
        Properties = new List<ConnectorPropertyValue>()
        {
            new ConnectorPropertyValue()
            {
                Label = "ServerUrl",
                Value = ServerUrl
            },
            new ConnectorPropertyValue()
            {
                Label = "ViewStyle",
                Value = FeedViewStyle.HorizontalGrid
            }
        };
    }

    public JellyfinMostPlayed()
    {
        
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