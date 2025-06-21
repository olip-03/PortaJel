using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Spotify
{
    public class SpotifyServerConnector : IMediaServerConnector
    {
        public IMediaDataConnector AlbumData { get; set; } = null;
        public IMediaDataConnector ArtistData { get; set; } = null;
        public IMediaDataConnector SongData { get; set; } = null;
        public IMediaDataConnector PlaylistData { get; set; } = new SpotifyServerPlaylistConnector();
        public IMediaDataConnector Genre { get; set; } = null;
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
        public string Name { get; } = "Spotify";
        public string Description { get; } = "Enables connections to Spotify.";
        public string Image { get; } = "icon_spotify.png"; 
        public Dictionary<string, ConnectorProperty> Properties { get; set; } =new Dictionary<string, ConnectorProperty>
        {
            {
                "Username", new ConnectorProperty(
                    label: "Username",
                    description: "The username of the Spotify User",
                    value: "",
                    protectValue: false,
                    userVisible: true)
            },
            {
                "Password", new ConnectorProperty(
                    label: "Password",
                    description: "The Password of the Spotify User",
                    value: "",
                    protectValue: false,
                    userVisible: true)
            }
        };

        public SyncStatusInfo SyncStatus { get; set; } = new();
        public List<Action<CancellationToken>> StartSyncActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Action<CancellationToken>> AuthenticateActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AuthStatusInfo AuthStatus { get; set; } = new AuthStatusInfo();
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
        
        public SpotifyServerConnector(string username, string password)
        {
            Properties["Username"].Value = username;
            Properties["Password"].Value = password;
        }
        
        
        public string GetUsername()
        {
            return "";
        }
        
        public string GetPassword()
        {
            return "";
        }
        
        public string GetAddress()
        {
            return "";
        }

        public string GetProfileImageUrl()
        {
            throw new NotImplementedException();
        }

        public UserCredentials GetUserCredentials()
        {
            return new UserCredentials(Properties["Url"].Value.ToString(), Properties["Username"].Value.ToString(), string.Empty, Properties["Password"].Value.ToString(), string.Empty, string.Empty);
        }
        
        public MediaServerConnection GetConnectionType()
        {
            return MediaServerConnection.Spotify;
        }

        public SpotifyServerConnector() 
        {
            
        }
    }
}
