using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.FS;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Services.Jellyfin.Feeds;
using Portajel.Connections.Services.MusicBrainz;
using Portajel.Connections.Services.Spotify;

namespace Portajel.Connections.Definitions;

public record ConnectorDefinition<T>(Func<IDbConnector, ConnectorProperties, T> Factory, Type Type);

public class ConnectorDefinitions
{
    public static Dictionary<string, ConnectorDefinition<IMediaServerConnector>> ServerConnectorDefinitions
    {
        get;
        private set;
    } = new()
    {
        {
            "JellyfinServerConnector",
            new((IDbConnector database, ConnectorProperties properties) => new JellyfinServerConnector(database, properties), typeof(JellyfinServerConnector))
        },
        {
            "MusicBrainzConnector",
            new((IDbConnector database, ConnectorProperties properties) => new MusicBrainzConnector(), typeof(MusicBrainzConnector))
        },
        {
            "FileSystemConnector",
            new((IDbConnector database, ConnectorProperties properties) => new FileSystemConnector(), typeof(FileSystemConnector))
        },
        {
            "SpotifyServerConnector",
            new((IDbConnector database, ConnectorProperties properties) => new SpotifyServerConnector(), typeof(SpotifyServerConnector))
        },
    };

    public static Dictionary<string, ConnectorDefinition<IMediaFeed>> MediaFeedDefinitions { get; private set; } = new()
    {
        {
            "JellyfinRecentlyListened",
            new((IDbConnector database, ConnectorProperties properties) => new JellyfinRecentlyListened(database, properties), typeof(JellyfinRecentlyListened))
        },
        {
            "JellyfinRecentlyAdded",
            new((IDbConnector database, ConnectorProperties properties) => new JellyfinRecentlyAdded(database, properties), typeof(JellyfinRecentlyAdded))
        },
        { "JellyfinMostPlayed", new((IDbConnector database, ConnectorProperties properties) => new JellyfinMostPlayed(database, properties), typeof(JellyfinMostPlayed)) }
    };
}