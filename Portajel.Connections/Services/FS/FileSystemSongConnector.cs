using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using SQLite;
using Portajel.Connections.Structs;
using Portajel.Connections.Database;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Services.FS;

public class FileSystemSongConnector : IMediaDataConnector
{
    private SQLiteAsyncConnection _database = null;
    private IMediaDataConnector _mediaDataConnectorImplementation;
    public MediaType MediaType => MediaType.Song;

    public FileSystemSongConnector(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public SyncStatusInfo SyncStatusInfo { get; set; }

    public void SetSyncStatusInfo(TaskStatus status, int percentage)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseData[]> GetAllAsync(
        int? limit = null, 
        int startIndex = 0, 
        bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album, 
        SortOrder setSortOrder = SortOrder.Ascending,
        Guid? parentId = null,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default
    )
    {
        // Implementation to fetch all songs
        return await Task.FromResult(Array.Empty<SongData>());
    }

    public async Task<BaseData> GetAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        // Implementation to fetch a specific SongData by its ID
        return await Task.FromResult(SongData.Empty);
    }

    public Task<BaseData[]> GetSimilarAsync(Guid id, int setLimit, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseData[]> GetSimilarAsync(Guid id, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        // Implementation to fetch similar songs to the specified one
        return await Task.FromResult(new SongData[0]);
    }
    
    public async Task<int> GetTotalCountAsync(bool? getFavourite = null, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        // Implementation to get the total count of songs
        return await Task.FromResult(0);
    }

    public Task<bool> DeleteAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid[] id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddRange(BaseData[] musicItems, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}