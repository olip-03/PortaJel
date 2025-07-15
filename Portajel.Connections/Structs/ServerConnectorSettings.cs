using Newtonsoft.Json;
using Portajel.Connections.Services;
using Portajel.Connections.Services.FS;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Services.Spotify;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;

namespace Portajel.Connections.Structs;

public class ServerConnectorSettings
{
    public List<ServerSettings> Servers { get; set; } = new();
}

public class ServerSettings
{
    public ConnectorProperties Properties { get; set; } = new();
    public string Id { get; set; } = "";
    public List<BaseMediaFeed> MediaFeeds { get; set; } = new();

    public ServerSettings()
    {
        
    }
    public ServerSettings(IMediaServerConnector? mediaServerConnector)
    {
        if (mediaServerConnector == null)
        {
            return;
        }
        Id = mediaServerConnector.Id;
        Properties = mediaServerConnector.Properties;
        MediaFeeds = mediaServerConnector.Feeds == null ? new List<BaseMediaFeed>() : mediaServerConnector.Feeds.Values.Select(f => new BaseMediaFeed(f)).ToList();
    }
}