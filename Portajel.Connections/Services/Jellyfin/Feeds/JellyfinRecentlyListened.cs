using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinRecentlyListened(IDbConnector database, string serverUrl, bool isEnabled) : IMediaFeed
{
    public string Id { get; } = "JellyfinRecentlyListened";
    public string Name { get; } = "Recently Listened";
    public string Description { get; } = "Music you've recently played";
    public string ServerUrl { get; } = serverUrl;
    public bool IsEnabled { get; set; } = isEnabled;
    private readonly IDbConnector? _database = database;

    public BaseData[] GetFrom(int itemIndex, int amount)
    {
        return database.Connectors.Album.GetAll(amount, itemIndex, setSortTypes: ItemSortBy.PlayCount);
    }

    public int Total()
    {
        return database.Connectors.Album.GetTotalCount();
    }
}