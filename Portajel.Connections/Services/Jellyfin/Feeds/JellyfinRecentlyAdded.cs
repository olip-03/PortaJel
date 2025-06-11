using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin.Feeds;

public class JellyfinRecentlyAdded(IDbConnector database, string serverUrl, bool isEnabled) : IMediaFeed
{
    public string Id { get; }= "JellyfinRecentlyAdded";
    public string Name { get; } = "Recently Added";

    public string Description => $"Most recently added music from {ServerUrl}.";
    public string ServerUrl { get; } = null!;
    public bool IsEnabled { get; set; }
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