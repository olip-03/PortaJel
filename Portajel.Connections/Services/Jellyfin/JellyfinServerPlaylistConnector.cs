using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Services.Jellyfin;

public class JellyfinServerPlaylistConnector(JellyfinApiClient api, JellyfinSdkSettings clientSettings, UserDto user)
    : IMediaDataConnector, IMediaPlaylistInterface
{
    public SyncStatusInfo SyncStatusInfo { get; set; } = new();

    public MediaType MediaType => MediaType.Playlist;

    public void SetSyncStatusInfo(TaskStatus status, int percentage)
    {
        SyncStatusInfo.TaskStatus = status;
        SyncStatusInfo.StatusPercentage = percentage;
    }

    public async Task<BaseData[]> GetAllAsync(int? limit = null, int startIndex = 0, bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album, SortOrder setSortOrder = SortOrder.Ascending, Guid?[] includeIds = null,
        Guid?[] excludeIds = null, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        var playlistResult = await api.Items.GetAsync(c =>
        {
            c.QueryParameters.UserId = user.Id;
            c.QueryParameters.SortBy = [setSortTypes];
            c.QueryParameters.SortOrder = [setSortOrder];
            c.QueryParameters.IsFavorite = getFavourite;
            c.QueryParameters.IncludeItemTypes = [BaseItemKind.Playlist];
            c.QueryParameters.Limit = limit;
            c.QueryParameters.StartIndex = startIndex;
            c.QueryParameters.Recursive = true;
            c.QueryParameters.EnableImages = true;
            c.QueryParameters.EnableTotalRecordCount = true;
            c.QueryParameters.Fields = [ItemFields.Path];
        }, cancellationToken).ConfigureAwait(false);
        if (playlistResult == null || cancellationToken.IsCancellationRequested) return [];
        if (playlistResult.Items == null || cancellationToken.IsCancellationRequested) return [];
        return playlistResult.Items.Select(p => PlaylistData.Builder(p, clientSettings.ServerUrl)).ToArray();
    }
    
    public async Task<BaseData> GetAsync(Guid id, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        // Implementation to fetch a specific PlaylistData by its ID
        var playlistQueryResult = api.Items.GetAsync(c =>
        {
            c.QueryParameters.UserId = user.Id;
            c.QueryParameters.Ids = [id];
            c.QueryParameters.Recursive = true;
            c.QueryParameters.EnableImages = true;
            c.QueryParameters.Fields = [ItemFields.Path];
        }, cancellationToken);
        var playlistSongResult = api.Items.GetAsync(c =>
        {
            c.QueryParameters.UserId = user.Id;
            c.QueryParameters.ParentId = id;
            c.QueryParameters.Fields =
                [ItemFields.ParentId, ItemFields.Path, ItemFields.MediaStreams, ItemFields.CumulativeRunTimeTicks];
            c.QueryParameters.EnableImages = true;
        }, cancellationToken);
        await Task.WhenAll(playlistQueryResult, playlistSongResult);
        if (playlistQueryResult.Result?.Items == null) return PlaylistData.Empty;
        if (playlistSongResult.Result?.Items == null) return PlaylistData.Empty;
        var songData = playlistSongResult.Result?.Items.Select(song => SongData.Builder(song, clientSettings.ServerUrl))
            .ToArray();
        return await Task.FromResult(PlaylistData.Empty);
    }

    public Task<BaseData[]> GetSimilarAsync(Guid id, int setLimit, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetTotalCountAsync(bool? getFavourite = null, string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        BaseItemDtoQueryResult serverResults = await api.Items.GetAsync(c =>
        {
            c.QueryParameters.UserId = user.Id;
            c.QueryParameters.IsFavorite = getFavourite;
            c.QueryParameters.SortBy = [ItemSortBy.Name];
            c.QueryParameters.SortOrder = [SortOrder.Descending];
            c.QueryParameters.IncludeItemTypes = [BaseItemKind.Playlist];
            c.QueryParameters.Limit = 1;
            c.QueryParameters.StartIndex = 0;
            c.QueryParameters.Recursive = true;
            c.QueryParameters.EnableImages = true;
            c.QueryParameters.EnableTotalRecordCount = true;
        }, cancellationToken).ConfigureAwait(false);
        return serverResults?.TotalRecordCount ?? 0;
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