using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinMostPlayed : IMediaFeed
{
    private readonly IDbConnector? _database;
    public string Id { get; } = "JellyfinMostPlayed";
    public string Name { get; } = "Most Played";
    public string Description { get; } = "Most played from this library.";
    public string ServerUrl { get; }
    public bool IsEnabled { get; set; }
    public List<ConnectorProperty> Properties { get; set; }
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return _database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.DateCreated);
    }
    public int Total()
    {
        return _database.Connectors.Album.GetTotalCount();
    }

    public JellyfinMostPlayed(IDbConnector database, string serverUrl, bool isEnabled)
    {
        _database = database;
        ServerUrl = serverUrl;
        IsEnabled = isEnabled;
        Properties = new List<ConnectorProperty>()
        {
            new ConnectorProperty()
            {
                Label = "ServerUrl",
                Value = ServerUrl
            },
            new ConnectorProperty()
            {
                Label = "ViewStyle",
                Value = FeedViewStyle.HorizontalGrid
            }
        };
    }
}