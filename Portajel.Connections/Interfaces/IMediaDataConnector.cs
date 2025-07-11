using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Interfaces;

public interface IMediaDataConnector
{
    public MediaType MediaType { get; }
    public SyncStatusInfo SyncStatusInfo { get; set; }
    public void SetSyncStatusInfo(
        TaskStatus? status = null, 
        int? percentage = null,
        int? serverItemTotal = null,
        int? serverItemCount = null,
        int? DbFoundTotal = null
    )
    {
        if(status!=null) SyncStatusInfo.TaskStatus = status.Value;
        if(percentage!=null) SyncStatusInfo.StatusPercentage = percentage.Value;
        if (serverItemTotal != null) SyncStatusInfo.ServerItemTotal = serverItemTotal.Value;
        if (serverItemCount != null) SyncStatusInfo.ServerItemCount = serverItemCount.Value;
        if (DbFoundTotal != null) SyncStatusInfo.DbFoundTotal = DbFoundTotal.Value;
    }
    public void SetSyncStatusInfo(
        SyncStatusInfo syncStatus
    )
    {
        SyncStatusInfo = syncStatus;
    }
    Task<BaseData[]> GetAllAsync(
        int? limit = null, 
        int startIndex = 0, 
        bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album, 
        SortOrder setSortOrder = SortOrder.Ascending,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default
    );
    Task<BaseData> GetAsync(
        Guid id, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default);
    Task<BaseData[]> GetSimilarAsync(
        Guid id, 
        int setLimit, 
        string serverUrl = "",
        CancellationToken cancellationToken = default
    );
    Task<int> GetTotalCountAsync( 
        bool? getFavourite = null, 
        string serverUrl = "",
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteAsync(
        Guid id, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteAsync(
        Guid[] id, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default
    );
    Task<bool> AddRange(
        BaseData[] musicItems, 
        CancellationToken cancellationToken = default
    );
}