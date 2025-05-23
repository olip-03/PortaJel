using System.Diagnostics;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using SQLite;
using System.Linq;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Database
{
    public class DatabaseSongConnector : IDbItemConnector
    {
        private readonly SQLiteConnection _database;
        public MediaTypes MediaType { get; set; } = MediaTypes.Song;
        public DatabaseSongConnector(SQLiteConnection database)
        {
            _database = database;
        }

        public BaseData[] GetAll(
            int? limit = null,
            int startIndex = 0,
            bool? getFavourite = null,
            ItemSortBy setSortTypes = ItemSortBy.Album,
            SortOrder setSortOrder = SortOrder.Ascending,
            Guid?[]? includeIds = null,
            Guid?[]? excludeIds = null,
            CancellationToken cancellationToken = default)
        {
            // Start with base query
            var query = _database.Table<SongData>();

            // Set default limit if null
            limit ??= _database.Table<SongData>().Count();

            // Apply includeIds filter
            if (includeIds != null && includeIds.Any())
            {
                query = query.Where(song => includeIds.Contains(song.Id));
            }

            // Apply excludeIds filter - FIXED the condition that was checking includeIds instead of excludeIds
            if (excludeIds != null && excludeIds.Any())
            {
                query = query.Where(song => !excludeIds.Contains(song.Id));
            }

            // Apply getFavourite filter
            if (getFavourite.HasValue)
            {
                query = query.Where(song => song.IsFavourite == getFavourite.Value);
            }

            // Handle special case for Random sorting
            if (setSortTypes == ItemSortBy.Random)
            {
                var allSongs = query.ToList();
                return allSongs
                    .OrderBy(_ => Guid.NewGuid())
                    .Skip(startIndex)
                    .Take((int)limit)
                    .ToArray();
            }

            // Apply sorting for non-random cases
            switch (setSortTypes)
            {
                case ItemSortBy.DateCreated:
                    query = setSortOrder == SortOrder.Ascending
                        ? query.OrderBy(song => song.DateAdded)
                        : query.OrderByDescending(song => song.DateAdded);
                    break;
                case ItemSortBy.DatePlayed:
                    query = setSortOrder == SortOrder.Ascending
                        ? query.OrderBy(song => song.DatePlayed)
                        : query.OrderByDescending(song => song.DatePlayed);
                    break;
                case ItemSortBy.Name:
                    query = setSortOrder == SortOrder.Ascending
                        ? query.OrderBy(song => song.Name)
                        : query.OrderByDescending(song => song.Name);
                    break;
                case ItemSortBy.PlayCount:
                    query = setSortOrder == SortOrder.Ascending
                        ? query.OrderBy(song => song.PlayCount)
                        : query.OrderByDescending(song => song.PlayCount);
                    break;
                default:
                    query = setSortOrder == SortOrder.Ascending
                        ? query.OrderBy(song => song.Name)
                        : query.OrderByDescending(song => song.Name);
                    break;
            }

            // Apply pagination and execute query
            return query
                .Skip(startIndex)
                .Take((int)limit)
                .ToArray();
        }

        public BaseData Get(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var song = _database.Table<SongData>().Where(song => song.Id == id).FirstOrDefault();
            if (song == null) return SongData.Empty;
            return song;
        }
        public bool Contains(Guid id, CancellationToken cancellationToken = default)
        {
            var SongDataExists = _database.Table<SongData>()
                .Where(song => song.Id == id)
                .Count() > 0;
            return SongDataExists;
        }
        public int GetTotalCount(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default)
        {
            var query = _database.Table<SongData>();
            if (getFavourite == true)
                query = query.Where(song => song.IsFavourite);

            return query.Count();
        }

        public bool Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Delete associated SongDatas
                var SongDatas = _database.Table<SongData>().Where(s => s.Id == id).ToList();
                foreach (var song in SongDatas)
                {
                    _database.Delete(song);
                    Trace.WriteLine($"Deleted song with ID {song.Id} associated with AlbumData ID {id}.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error deleting song with ID {id}: {ex.Message}");
                return false; // Deletion failed
            }
        }

        public bool DeleteRange(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var id in ids)
                {
                    // Find the album
                    var song = _database.Table<SongData>().FirstOrDefault(s => s.Id == id);
                    if (song == null)
                    {
                        Trace.WriteLine($"song with ID {id} not found.");
                        return false; // Stop if any AlbumData is not found
                    }
                    _database.Delete(song);
                    Trace.WriteLine($"Deleted song with ID {id}.");
                }
                return true; // All deletions succeeded
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error deleting albums: {ex.Message}");
                return false; // Deletion failed for one or more
            }
        }

        public bool Insert(
            BaseData musicItem,
            CancellationToken cancellationToken = default)
        {
            if (musicItem is not SongData SongData) return false;
            _database.InsertOrReplace(SongData, SongData.GetType());
            return true;
        }

        public bool InsertRange(
            BaseData[] musicItems,
            CancellationToken cancellationToken = default)
        {
            Parallel.ForEach(musicItems, song =>
            {
                _database.InsertOrReplace(song);
            });
            return true;
        }
    }
}