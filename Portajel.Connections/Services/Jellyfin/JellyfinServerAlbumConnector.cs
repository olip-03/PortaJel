﻿using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin
{
    public class JellyfinServerAlbumConnector(JellyfinApiClient api, JellyfinSdkSettings clientSettings, UserDto user)
        : IMediaDataConnector
    {
        public SyncStatusInfo SyncStatusInfo { get; set; } = new();
        public MediaTypes MediaType => MediaTypes.Album;
        public async Task<BaseData[]> GetAllAsync(
            int? limit = null, 
            int startIndex = 0,
            bool? getFavourite = null,
            ItemSortBy setSortTypes = ItemSortBy.DateCreated, 
            SortOrder setSortOrder = SortOrder.Descending,
            Guid?[] includeIds = null,
            Guid?[] excludeIds = null, 
            string serverUrl = "", 
            CancellationToken cancellationToken = default)
        {
            BaseItemDtoQueryResult? serverResults = await api.Items.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.IsFavorite = getFavourite;
                c.QueryParameters.SortBy = [setSortTypes];
                c.QueryParameters.SortOrder = [setSortOrder];
                c.QueryParameters.IncludeItemTypes = [BaseItemKind.MusicAlbum];
                c.QueryParameters.ExcludeItemIds = excludeIds;
                c.QueryParameters.Limit = limit;
                c.QueryParameters.StartIndex = startIndex;
                c.QueryParameters.Recursive = true;
                c.QueryParameters.Fields = [ItemFields.DateCreated, ItemFields.DateLastSaved, ItemFields.DateLastMediaAdded, ItemFields.ProviderIds];
                c.QueryParameters.EnableImages = true;
                c.QueryParameters.EnableTotalRecordCount = true;
            }, cancellationToken).ConfigureAwait(false);
            if (serverResults == null) return [];
            if (serverResults.Items == null) return [];
            return serverResults.Items.Select(dto => AlbumData.Builder(dto, clientSettings.ServerUrl)).ToArray();
        }
        public async Task<BaseData> GetAsync(Guid id, string serverUrl = "",
            CancellationToken cancellationToken = default)
        {
            var albumQueryResult = api.Items.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.Ids = [id];
                c.QueryParameters.IncludeItemTypes = [BaseItemKind.MusicAlbum];
                c.QueryParameters.Recursive = true;
                c.QueryParameters.EnableImages = true;
            }, cancellationToken);
            var songQueryResult = api.Items.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.IncludeItemTypes = [BaseItemKind.Audio];
                c.QueryParameters.SortBy = [ItemSortBy.Album, ItemSortBy.SortName];
                c.QueryParameters.Fields =
                [
                    ItemFields.ParentId, ItemFields.Path, ItemFields.MediaStreams, ItemFields.CumulativeRunTimeTicks,
                    ItemFields.DateCreated
                ];
                c.QueryParameters.SortOrder = [SortOrder.Ascending];
                c.QueryParameters.ParentId = id;
                c.QueryParameters.Recursive = true;
                c.QueryParameters.EnableImages = true;
            }, cancellationToken);
            await Task.WhenAll(albumQueryResult, songQueryResult).ConfigureAwait(false);
            
            if (albumQueryResult.Result?.Items == null) return AlbumData.Empty;
            if (songQueryResult.Result?.Items == null) return AlbumData.Empty;
            
            BaseItemDto albumBaseItem = albumQueryResult.Result?.Items.FirstOrDefault();

               if (albumBaseItem?.ArtistItems == null) return AlbumData.Empty;
            BaseItemDtoQueryResult artistQueryResults = await api.Items.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.IncludeItemTypes = [BaseItemKind.MusicArtist];
                c.QueryParameters.Fields =
                    [ItemFields.ParentId, ItemFields.Path, ItemFields.MediaStreams, ItemFields.CumulativeRunTimeTicks];
                c.QueryParameters.SortOrder = [SortOrder.Ascending];
                c.QueryParameters.Ids = albumBaseItem.ArtistItems.Select(artist => artist.Id).ToArray();
                c.QueryParameters.Recursive = true;
                c.QueryParameters.EnableImages = true;
            }, cancellationToken).ConfigureAwait(false);
            if (artistQueryResults?.Items == null) return AlbumData.Empty; 
            var songBaseItemArray = songQueryResult.Result?.Items.ToArray();
            var artistBaseItemArray = artistQueryResults.Items.ToArray();
            if (songBaseItemArray != null)
            {
                var songData = songBaseItemArray.Select(item => SongData.Builder(item, clientSettings.ServerUrl))
                    .OrderBy(song => song.IndexNumber).ThenBy(song => song.DiskNumber).ToArray();
                return AlbumData.Builder(albumBaseItem, clientSettings.ServerUrl, songDataItems: songData);
            }
            return AlbumData.Empty;
        }
        public async Task<BaseData[]> GetSimilarAsync(
            Guid id, 
            int setLimit, 
            string serverUrl = "",
            CancellationToken cancellationToken = default)
        {
            BaseItemDtoQueryResult result = await api.Albums[id].Similar.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.Limit = setLimit;
            }, cancellationToken).ConfigureAwait(false);
            if (result?.Items == null) return [];
            return result.Items.Select(dto => AlbumData.Builder(dto, clientSettings.ServerUrl)).ToArray();
        }
        public async Task<int> GetTotalCountAsync(
            bool? getFavourite = null, 
            string serverUrl = "",
            CancellationToken cancellationToken = default)
        {
            BaseItemDtoQueryResult serverResults = await api.Items.GetAsync(c =>
            {
                c.QueryParameters.UserId = user.Id;
                c.QueryParameters.IsFavorite = getFavourite;
                c.QueryParameters.SortBy = [ItemSortBy.Name];
                c.QueryParameters.SortOrder = [SortOrder.Descending];
                c.QueryParameters.IncludeItemTypes = [BaseItemKind.MusicAlbum];
                c.QueryParameters.Limit = 1;
                c.QueryParameters.StartIndex = 0;
                c.QueryParameters.Recursive = true;
                c.QueryParameters.EnableImages = true;
                c.QueryParameters.EnableTotalRecordCount = true;
            }, cancellationToken).ConfigureAwait(false);
            return serverResults?.TotalRecordCount ?? 0;
        }

        public Task<bool> DeleteAsync(
            Guid id, 
            string serverUrl = "", 
            CancellationToken cancellationToken = default)
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
}