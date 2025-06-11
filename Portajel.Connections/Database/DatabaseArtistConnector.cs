using System.Diagnostics;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using SQLite;

namespace Portajel.Connections.Services.Database;

// ReSharper disable once CoVariantArrayConversion
public class DatabaseArtistConnector : IDbItemConnector
{
    private readonly SQLiteConnection _database;
    public MediaTypes MediaType { get; set; } = MediaTypes.Artist;
    public DatabaseArtistConnector(SQLiteConnection database)
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
        List<ArtistData> filteredCache = [];
        limit ??= _database.Table<ArtistData>().Count();
        switch (setSortTypes)
        {
            case ItemSortBy.DateCreated:
                filteredCache.AddRange(_database.Table<ArtistData>()
                    .OrderBy(album => album.DateAdded)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            case ItemSortBy.Name:
                filteredCache.AddRange(_database.Table<ArtistData>()
                    .OrderBy(album => album.Name)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            case ItemSortBy.Random:
                var firstTake = _database.Table<ArtistData>().ToList();
                filteredCache = firstTake
                    .OrderBy(AlbumData => Guid.NewGuid())
                    .Skip(startIndex)
                    .Take((int)limit)
                    .ToList();
                break;
            default:
                filteredCache.AddRange(_database.Table<ArtistData>()
                    .OrderBy(album => album.Name)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
        }
        return filteredCache.ToArray();
    }
    
    public BaseData Get(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return _database.Table<ArtistData>().Where(artist => artist.Id == id).FirstOrDefault();
    }
    public bool Contains(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var artistExists = _database.Table<ArtistData>()
            .Where(artist => artist.Id == id)
            .Count() > 0;
        return artistExists;
    }
    public int GetTotalCount(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default)
    {
        var query = _database.Table<ArtistData>();
        if (getFavourite == true)
            query = query.Where(album => album.IsFavourite);

        return query.Count();
    }

    public bool Delete(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        try
        {
            // Delete associated artists (if applicable)
            var artists = _database.Table<ArtistData>().Where(a => a.Id == id).ToList();
            foreach (var artist in artists)
            {
                _database.Delete(artist);
                Trace.WriteLine($"Deleted ArtistData with ID {artist.Id} associated with AlbumData ID {id}.");
            }
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting ArtistData with ID {id}: {ex.Message}");
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
                var artist = _database.Table<ArtistData>().FirstOrDefault(a => a.Id == id);
                if (artist == null)
                {
                    Trace.WriteLine($"ArtistData with ID {id} not found.");
                    return false; // Stop if any AlbumData is not found
                }
                _database.Delete(artist);
                Trace.WriteLine($"Deleted ArtistData with ID {id}.");
            }
            return true; // All deletions succeeded
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting artist: {ex.Message}");
            return false; // Deletion failed for one or more
        }
    }

    public bool Insert(
            BaseData musicItem,
            CancellationToken cancellationToken = default)
    {
        if (musicItem is not ArtistData artist) return false;
        _database.InsertOrReplace(artist);
        return true;
    }

    public bool InsertRange(
            BaseData[] musicItems,
            CancellationToken cancellationToken = default)
    {
        Parallel.ForEach(musicItems, artist =>
        {
            _database.InsertOrReplace(artist);
        });
        return true;
    }
}