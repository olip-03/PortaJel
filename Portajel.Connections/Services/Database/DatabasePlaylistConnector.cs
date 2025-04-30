using System.Diagnostics;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using SQLite;

namespace Portajel.Connections.Services.Database;

public class DatabasePlaylistConnector : IDbItemConnector, IMediaPlaylistInterface
{
    private readonly SQLiteConnection _database = null;
    public MediaTypes MediaType { get; set; } = MediaTypes.Playlist;
    public DatabasePlaylistConnector(SQLiteConnection database)
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
        limit ??= 50;
        List<PlaylistData> filteredCache = [];
        filteredCache.AddRange(_database.Table<PlaylistData>()
            .OrderByDescending(playlist => playlist.Name)
            .Take(limit.Value).ToList());
        return filteredCache.ToArray();
    }

    public BaseData Get(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return _database.Table<PlaylistData>().Where(p => p.Id == id).FirstOrDefault();
    }

    public int GetTotalCount(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default)
    {
        var query = _database.Table<PlaylistData>();
        if (getFavourite == true)
            query = query.Where(song => song.IsFavourite);
        return query.Count();
    }
    public bool Contains(Guid id, CancellationToken cancellationToken = default)
    {
        var playlistExists = _database.Table<PlaylistData>()
            .Where(playlist => playlist.Id == id)
            .Count() > 0;
        return playlistExists;
    }
    public Task<bool> RemovePlaylistItemAsync(Guid playlistId, Guid songId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovePlaylistItem(Guid playlistId, Guid songId, int newIndex, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool Delete(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        try
        {
            // Delete associated playlists
            var playlists = _database.Table<PlaylistData>().Where(p => p.Id == id).ToList();
            foreach (var playlist in playlists)
            {
                _database.Delete(playlist);
                Trace.WriteLine($"Deleted PlaylistData with ID {playlist.Id} associated with AlbumData ID {id}.");
            }
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting PlaylistData with ID {id}: {ex.Message}");
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
                var playlist = _database.Table<PlaylistData>().FirstOrDefault(p => p.Id == id);
                if (playlist == null)
                {
                    Trace.WriteLine($"PlaylistData with ID {id} not found.");
                    return false; // Stop if any AlbumData is not found
                }
                _database.Delete(playlist);
                Trace.WriteLine($"Deleted PlaylistData with ID {id}.");
            }
            return true; // All deletions succeeded
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting playlists: {ex.Message}");
            return false; // Deletion failed for one or more
        }
    }

    public bool Insert(
            BaseData musicItem,
            CancellationToken cancellationToken = default)
    {
        if (musicItem is not PlaylistData playlist) return false;
        _database.InsertOrReplace(playlist, playlist.GetType());
        return true;
    }

    public bool InsertRange(
            BaseData[] musicItems,
            CancellationToken cancellationToken = default)
    {
        Parallel.ForEach(musicItems, playlist =>
        {
            _database.InsertOrReplace(playlist);
        });
        return true;
    }

    public Task<bool> RemovePlaylistItemAsync(Guid playlistId, Guid songId, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovePlaylistItem(Guid playlistId, Guid songId, int newIndex, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovePlaylistItem(Guid playlistId, Guid songId, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}