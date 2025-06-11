using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinMostPlayed(IDbConnector database, string serverUrl, bool isEnabled) : IMediaFeed
{
    public string Id { get; } = "JellyfinMostPlayed";
    public string Name { get; } = "Recently Most Played";
    public string Description { get; } = "Most played from this library.";
    public string ServerUrl { get; } = serverUrl;
    public bool IsEnabled { get; set; } = isEnabled;
    private readonly IDbConnector? _database = database;
    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.DateCreated);
    }

    public int Total()
    {
        return database.Connectors.Album.GetTotalCount();
    }
}