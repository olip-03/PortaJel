using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using SQLite;

namespace Portajel.Connections.Services.Database;

public class DatabaseGenreConnector : IDbItemConnector
{
    private readonly SQLiteConnection _database = null;
    public MediaTypes MediaType { get; set; } = MediaTypes.Genre;
    public DatabaseGenreConnector(SQLiteConnection database)
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
        return [];
    }

    public BaseData Get(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return GenreData.Empty;
    }
    public bool Contains(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return false;
    }
    public int GetTotalCount(
            bool? getFavourite = null,
            CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public bool Delete(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return false;
    }

    public bool DeleteRange(
            Guid[] ids,
            CancellationToken cancellationToken = default)
    {
        return false;
    }

    public bool Insert(
            BaseData musicItem,
            CancellationToken cancellationToken = default)
    {
        return false;
    }

    public bool InsertRange(
            BaseData[] musicItems,
            CancellationToken cancellationToken = default)
    {
        return false;
    }
}