using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Interfaces
{
    public interface IDbConnector
    {
        public SQLiteConnection Database { get; }
        DbConnectors Connectors { get; }
        public BaseData[] Search(
            string searchTerm = "",
            int limit = 50,
            int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name,
            SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default);
    }
    public class DbConnectors(IDbItemConnector album, 
        IDbItemConnector artist, 
        IDbItemConnector genre, 
        IDbItemConnector playlist, 
        IDbItemConnector song)
    {
        public IDbItemConnector Album { get; } = album;
        public IDbItemConnector Artist { get; } = artist;
        public IDbItemConnector Genre { get; } = genre;
        public IDbItemConnector Playlist { get; } = playlist;
        public IDbItemConnector Song { get; } = song;
        public Dictionary<MediaCapabilities, IDbItemConnector> GetDataConnectors() => new()
        {
            { MediaCapabilities.Album, Album },
            { MediaCapabilities.Artist, Artist },
            { MediaCapabilities.Genre, Genre },
            { MediaCapabilities.Playlist, Playlist },
            { MediaCapabilities.Song, Song }
        };
    }
}
