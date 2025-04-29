using System.Diagnostics;
using Portajel.Connections.Enum;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Database;

// ReSharper disable once CoVariantArrayConversion
public class DatabaseAlbumConnector : IDbItemConnector
{
    private readonly SQLiteConnection _database;

    public MediaTypes MediaType { get; set; } = MediaTypes.Album;

    public DatabaseAlbumConnector(SQLiteConnection database)
    {
        _database = database;
    }
    public BaseData[] GetAll(
        int? limit = null, 
        int startIndex = 0,
        bool? getFavourite = null, 
        ItemSortBy setSortTypes = ItemSortBy.Album, 
        SortOrder setSortOrder = SortOrder.Descending, 
        Guid?[]? includeIds = null, 
        Guid?[]? excludeIds = null, 
        CancellationToken cancellationToken = default)
    {
        List<AlbumData> filteredCache = [];
        limit ??= _database.Table<AlbumData>().Count();
        switch (setSortTypes)
        {
            case ItemSortBy.DateCreated:
                filteredCache.AddRange(_database.Table<AlbumData>()
                    .OrderByDescending(album => album.DateAdded)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            case ItemSortBy.DatePlayed:
                filteredCache.AddRange(_database.Table<AlbumData>()
                    .OrderByDescending(album => album.DatePlayed)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            case ItemSortBy.Name:
                filteredCache.AddRange(_database.Table<AlbumData>()
                    .OrderByDescending(album => album.Name)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            case ItemSortBy.Random:
                var firstTake = _database.Table<AlbumData>().ToList();
                filteredCache = firstTake
                    .OrderBy(AlbumData => Guid.NewGuid())
                    .Skip(startIndex)
                    .Take((int)limit)
                    .ToList();
                break;
            case ItemSortBy.PlayCount:
                filteredCache.AddRange(
                    _database.Table<AlbumData>()
                    .OrderByDescending(album => album.PlayCount)
                    .Skip(startIndex)
                    .Take((int)limit).ToList());
                break;
            default:
                filteredCache.AddRange(_database.Table<AlbumData>()
                    .OrderByDescending(album => album.Name)
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
        // Filter the cache based on the provided parameters
        return _database.Table<AlbumData>().Where(album => album.Id == id).FirstOrDefault();
    }

    public bool Contains(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = Get(id, cancellationToken);
        return result != AlbumData.Empty;
    }

    public int GetTotalCount(
        bool? getFavourite = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _database.Table<AlbumData>();
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
            // Find the album
            var album = _database.Table<AlbumData>().FirstOrDefault(a => a.Id == id);
            if (album == null) return false;
            _database.Delete(album);
            Trace.WriteLine($"Deleted AlbumData with ID {id}.");
            return true; 
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting AlbumData with ID {id}: {ex.Message}");
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
                var album = _database.Table<AlbumData>().FirstOrDefault(a => a.Id == id);
                if (album == null)
                {
                    Trace.WriteLine($"AlbumData with ID {id} not found.");
                    return false; // Stop if any AlbumData is not found
                }
                _database.Delete(album);
                Trace.WriteLine($"Deleted AlbumData with ID {id}.");
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
        BaseData album,
        CancellationToken cancellationToken = default)
    {
        if (album is AlbumData a && a != null)
        {
            _database.InsertOrReplace(a);
        }
        return true;
    }

    public bool InsertRange(
        BaseData[] albums, 
        CancellationToken cancellationToken = default)
    {
        foreach (var baseMusicItem in albums)
        {
            if (baseMusicItem is AlbumData a && a != null)
            {
                _database.InsertOrReplace(a);
            }
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
        return true;
    }
}