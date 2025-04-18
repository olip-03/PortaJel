﻿using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces
{
    public interface IDbItemConnector
    {
        public MediaTypes MediaType { get; set; }
        Task<BaseMusicItem[]> GetAllAsync(
            int? limit = null,
            int startIndex = 0,
            bool? getFavourite = null,
            ItemSortBy setSortTypes = ItemSortBy.Album,
            SortOrder setSortOrder = SortOrder.Ascending,
            Guid?[]? includeIds = null,
            Guid?[]? excludeIds = null,
            CancellationToken cancellationToken = default);
        Task<BaseMusicItem> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<bool> Contains(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<int> GetTotalCountAsync(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteRangeAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default);
        Task<bool> InsertAsync(
            BaseMusicItem musicItem,
            CancellationToken cancellationToken = default);
        Task<bool> InsertRangeAsync(
            BaseMusicItem[] musicItems,
            CancellationToken cancellationToken = default);
    }
}
