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
        public DbConnectors Connectors { get; }
        public BaseData[] Search(
            string searchTerm = "",
            int limit = 50,
            int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name,
            SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default);
    }

    public class DbConnectors : Dictionary<MediaCapabilities, IDbItemConnector>
    {
        public IDbItemConnector Album { get; } 
        public IDbItemConnector Artist { get; } 
        public IDbItemConnector Genre { get; } 
        public IDbItemConnector Playlist { get; }
        public IDbItemConnector Song { get; } 

        public DbConnectors(IDbItemConnector album,
            IDbItemConnector artist,
            IDbItemConnector genre,
            IDbItemConnector playlist,
            IDbItemConnector song)
        {
            Album = album;
            Add(MediaCapabilities.Album, Album);
            
            Artist = artist;
            Add(MediaCapabilities.Artist, Artist);
            
            Genre = genre;
            Add(MediaCapabilities.Genre, Genre);
            
            Playlist = playlist;
            Add(MediaCapabilities.Playlist, Playlist);
            
            Song = song;
            Add(MediaCapabilities.Song, Song);
        }
    }
}
