using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.FS;

public class FileSystemConnector : IMediaServerConnector
{
    private SQLiteAsyncConnection _database = null;

    public IMediaDataConnector AlbumData { get; set; }
    public IMediaDataConnector ArtistData { get; set; }
    public IMediaDataConnector SongData { get; set; }
    public IMediaDataConnector PlaylistData { get; set; }
    public IMediaDataConnector Genre { get; set; }
    public IFeedConnector? Feeds { get; }
    public Dictionary<string, IMediaDataConnector> DataConnectors => new()
    {
        { "Album", AlbumData },
        { "Artist", ArtistData },
        { "Song", SongData },
        { "Playlist", PlaylistData },
        { "Genre", Genre }
    };
    public Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; } = new()
    {
        { MediaCapabilities.Album, true },
        { MediaCapabilities.Artist, true },
        { MediaCapabilities.Song, true },
        { MediaCapabilities.Playlist, true },
        { MediaCapabilities.Genre, true }
    };
    public string Name { get; } = "File System";
    public string Description { get; } = "Enables connections to a local file system.";
    public string Image { get; } = "icon-spotify.png"; 
    public Dictionary<string, ConnectorProperty> Properties { get; set; } =new Dictionary<string, ConnectorProperty>
    {
        {
            "Paths", new ConnectorProperty(
                label: "Music Directories",
                description: "The directories of music files in your file system.",
                value: new List<string>(),
                protectValue: false,
                userVisible: true)
        }
    };
    
    public SyncStatusInfo SyncStatus { get; set; } = new();
    public List<Action<CancellationToken>> StartSyncActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public List<Action<CancellationToken>> AuthenticateActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public FileSystemConnector()
    {
        
    }
    
    public FileSystemConnector(SQLiteAsyncConnection database, List<string> paths)
    {
        Properties["Paths"].Value = paths;
        _database = database;
        
        AlbumData = new FileSystemAlbumConnector(_database);
        ArtistData = new FileSystemArtistConnector(_database);
        SongData = new FileSystemSongConnector(_database);
        PlaylistData = new FileSystemPlaylistConnector(_database);
        Genre = new FileSystemGenreConnector(_database);
    }
    public AuthStatusInfo AuthStatus { get; set; } = new AuthStatusInfo();
    public Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(AuthStatusInfo.Unneccesary());
    }
    
    public Task<bool> UpdateDb(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SetIsFavourite(Guid id, bool isFavourite, string serverUrl)
    {
        throw new NotImplementedException();
    }
    
    public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
        ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Array.Empty<BaseData>());
    }

    public string GetUsername()
    {
        return null;
    }
    
    public string GetPassword()
    {
        return null;
    }
    
    public string GetAddress()
    {
        return "http://localhost:5000";
    }

    public string GetProfileImageUrl()
    {
        return null;
    }

    public UserCredentials GetUserCredentials()
    {
        return new UserCredentials(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }

    public MediaServerConnection GetConnectionType()
    {
        return MediaServerConnection.Filesystem;
    }
}