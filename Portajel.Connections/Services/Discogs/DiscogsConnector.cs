using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Discogs;

// https://github.com/David-Desmaisons/DiscogsClient

public class DiscogsConnector : IMediaServerConnector
{
    public IMediaDataConnector AlbumData { get; set; }
    public IMediaDataConnector ArtistData { get; set; }
    public IMediaDataConnector SongData { get; set; }
    public IMediaDataConnector PlaylistData { get; set; }
    public IMediaDataConnector Genre { get; set; }
    public IFeedConnector? Feeds { get; }
    public Dictionary<string, IMediaDataConnector> GetDataConnectors()=> new()
    {
        { "Album", AlbumData },
        { "Artist", ArtistData },
        { "Song", SongData },
        { "Playlist", PlaylistData },
        { "Genre", Genre }
    };
    public string Name { get; } = "Discogs";
    public string Description { get; } = "Enables connections to a Discogs.";
    public string Image { get; } = "icon-spotify.png";
    public Dictionary<string, ConnectorProperty> Properties { get; set; }
    public SyncStatusInfo SyncStatus { get; set; } = new();
    public List<Action<CancellationToken>> StartSyncActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public AuthStatusInfo AuthStatus { get; set; } = new AuthStatusInfo();
    public List<Action<CancellationToken>> AuthenticateActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; } = new ()
    {
        { MediaCapabilities.Album, true },
        { MediaCapabilities.Artist, true },
        { MediaCapabilities.Song, true },
        { MediaCapabilities.Playlist, true },
        { MediaCapabilities.Genre, true }
    };

    public Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateDb(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetIsFavourite(Guid id, bool isFavourite, string serverUrl)
    {
        throw new NotImplementedException();
    }

    public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
        ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetUsername()
    {
        throw new NotImplementedException();
    }

    public string GetPassword()
    {
        throw new NotImplementedException();
    }

    public string GetAddress()
    {
        throw new NotImplementedException();
    }

    public string GetProfileImageUrl()
    {
        throw new NotImplementedException();
    }

    public UserCredentials GetUserCredentials()
    {
        throw new NotImplementedException();
    }

    public MediaServerConnection GetConnectionType()
    {
        throw new NotImplementedException();
    }
}