using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;
using Portajel.Connections.Database;

namespace Portajel.Connections.Services.Jellyfin;

public class JellyfinServerGenreConnector(JellyfinApiClient api, JellyfinSdkSettings clientSettings, UserDto user) : IMediaDataConnector
{
    public SyncStatusInfo SyncStatusInfo { get; set; } = new();
    public MediaTypes MediaType => MediaTypes.Genre;

    public void SetSyncStatusInfo(TaskStatus status, int percentage)
    {
        SyncStatusInfo.TaskStatus = status;
        SyncStatusInfo.StatusPercentage = percentage;
    }

    public Task<BaseData[]> GetAllAsync(int? limit = null, int startIndex = 0, bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album, SortOrder setSortOrder = SortOrder.Ascending, Guid?[] includeIds = null,
        Guid?[] excludeIds = null, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BaseData[]>([]);
    }

    public Task<BaseData> GetAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BaseData>(AlbumData.Empty);
    }

    public Task<BaseData[]> GetSimilarAsync(Guid id, int setLimit, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BaseData[]>([]);
    }

    public Task<int> GetTotalCountAsync(bool? getFavourite = null, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<int>(0);
    }

    public Task<bool> DeleteAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        return Task.FromResult<bool>(false);
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
