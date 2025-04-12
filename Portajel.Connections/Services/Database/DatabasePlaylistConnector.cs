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
    private readonly SQLiteAsyncConnection _database = null;
    public MediaTypes MediaType { get; set; } = MediaTypes.Playlist;
    public DatabasePlaylistConnector(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public async Task<BaseMusicItem[]> GetAllAsync(
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
        filteredCache.AddRange(await _database.Table<PlaylistData>()
            .OrderByDescending(playlist => playlist.Name)
            .Take(limit.Value).ToListAsync().ConfigureAwait(false));
        return filteredCache.Select(dbItem => new Playlist(dbItem)).ToArray();
    }

    public async Task<BaseMusicItem> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        PlaylistData playlistDbItem =
            await _database.Table<PlaylistData>().Where(p => p.Id == id).FirstOrDefaultAsync();
        var songData = await _database.Table<SongData>().Where(song => playlistDbItem.GetSongIds().Contains(song.Id)).ToArrayAsync();
        return new Playlist(playlistDbItem, songData);
    }

    public async Task<int> GetTotalCountAsync(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default)
    {
        var query = _database.Table<PlaylistData>();
        if (getFavourite == true)
            query = query.Where(song => song.IsFavourite);

        return await query.CountAsync();
    }
    public async Task<bool> Contains(Guid id, CancellationToken cancellationToken = default)
    {
        var playlistExists = await _database.Table<PlaylistData>()
            .Where(playlist => playlist.Id == id)
            .CountAsync() > 0;
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

    public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        try
        {
            // Delete associated playlists
            var playlists = await _database.Table<PlaylistData>().Where(p => p.Id == id).ToListAsync();
            foreach (var playlist in playlists)
            {
                await _database.DeleteAsync(playlist);
                Trace.WriteLine($"Deleted playlist with ID {playlist.Id} associated with album ID {id}.");
            }
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting playlist with ID {id}: {ex.Message}");
            return false; // Deletion failed
        }
    }

    public async Task<bool> DeleteRangeAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var id in ids)
            {
                // Find the album
                var playlist = await _database.Table<PlaylistData>().FirstOrDefaultAsync(p => p.Id == id);
                if (playlist == null)
                {
                    Trace.WriteLine($"Playlist with ID {id} not found.");
                    return false; // Stop if any album is not found
                }
                await _database.DeleteAsync(playlist);
                Trace.WriteLine($"Deleted playlist with ID {id}.");
            }
            return true; // All deletions succeeded
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error deleting playlists: {ex.Message}");
            return false; // Deletion failed for one or more
        }
    }

    public async Task<bool> InsertAsync(
            BaseMusicItem musicItem,
            CancellationToken cancellationToken = default)
    {
        if (musicItem is not Playlist playlist) return false;
        await _database.InsertOrReplaceAsync(playlist.GetBase, playlist.GetBase.GetType());
        return true;
    }

    public async Task<bool> InsertRangeAsync(
            BaseMusicItem[] musicItems,
            CancellationToken cancellationToken = default)
    {
        foreach (var p in musicItems)
        {
            if (p is not Playlist playlist) continue;
            await _database.InsertOrReplaceAsync(playlist.GetBase, playlist.GetBase.GetType());
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }
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