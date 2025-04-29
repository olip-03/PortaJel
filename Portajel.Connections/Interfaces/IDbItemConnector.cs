using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces
{
    public interface IDbItemConnector
    {
        public MediaTypes MediaType { get; set; }
        BaseData[] GetAll(
            int? limit = null,
            int startIndex = 0,
            bool? getFavourite = null,
            ItemSortBy setSortTypes = ItemSortBy.Album,
            SortOrder setSortOrder = SortOrder.Ascending,
            Guid?[]? includeIds = null,
            Guid?[]? excludeIds = null,
            CancellationToken cancellationToken = default);
        BaseData Get(
            Guid id,
            CancellationToken cancellationToken = default);
        bool Contains(
            Guid id,
            CancellationToken cancellationToken = default);
        int GetTotalCount(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default);
        bool Delete(
            Guid id,
            CancellationToken cancellationToken = default);
        bool DeleteRange(
            Guid[] ids,
            CancellationToken cancellationToken = default);
        bool Insert(
            BaseData musicItem,
            CancellationToken cancellationToken = default);
        bool InsertRange(
            BaseData[] musicItems,
            CancellationToken cancellationToken = default);
    }
}