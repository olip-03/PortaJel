using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Structs;
using SQLite;

namespace Portajel.Connections.Database;

public static class DatabaseTypeConverters
{
    public static BaseData GetTyped<T>(SQLiteConnection database, Guid id) where T : BaseData, new()
    {
        var item = database.Table<T>().FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            throw new InvalidOperationException($"Item with ID {id} not found in {typeof(T).Name} table.");
        }
        return item;
    }
    
    public static bool ContainsTyped<T>(SQLiteConnection database, Guid id) where T : BaseData, new()
    {
        return database.Table<T>().Any(x => x.Id == id);
    }
    
    public static int GetTypedTotalCount<T>(SQLiteConnection database, bool? getFavourite) where T : BaseData, new()
    {
        var query = database.Table<T>();
        if (getFavourite.HasValue)
        {
            query = query.Where(item => item.IsFavourite == getFavourite.Value);
        }
        return query.Count();
    }
    
    public static bool DeleteTyped<T>(SQLiteConnection database, Guid id) where T : BaseData, new()
    {
        return database.Delete<T>(id) > 0;
    }
    
    public static bool DeleteRangeTyped<T>(SQLiteConnection database, Guid[] ids) where T : BaseData, new()
    {
        var deletedCount = 0;
        foreach (var id in ids)
        {
            deletedCount += database.Delete<T>(id);
        }
        return deletedCount > 0;
    }
    
    public static bool InsertTyped<T>(SQLiteConnection database, BaseData musicItem) where T : BaseData, new()
    {
        return database.Insert((T)musicItem) > 0;
    }
    
    public static bool InsertRangeTyped<T>(SQLiteConnection database, BaseData[] musicItems) where T : BaseData, new()
    {
        return database.InsertAll(musicItems.Cast<T>()) > 0;
    }
    
    public static BaseData[] GetTypedAll<T>(
        SQLiteConnection database, 
        int? limit, 
        int startIndex, 
        bool? getFavourite, 
        ItemSortBy setSortTypes, 
        SortOrder setSortOrder, 
        Guid?[]? includeIds, 
        Guid?[]? excludeIds) where T : BaseData, new()
    {
        var query = database.Table<T>();
        
        if (includeIds != null && includeIds.Any())
        {
            query = query.Where(item => includeIds.Contains(item.Id));
        }
        
        if (excludeIds != null && excludeIds.Any())
        {
            query = query.Where(item => !excludeIds.Contains(item.Id));
        }
        
        if (getFavourite.HasValue)
        {
            query = query.Where(item => item.IsFavourite == getFavourite.Value);
        }
        
        query = ApplySorting(query, setSortTypes, setSortOrder);
        query = query.Skip(startIndex).Take(limit ?? 50);
        
        return query.ToArray().Cast<BaseData>().ToArray();
    }
    
    private static TableQuery<T> ApplySorting<T>(TableQuery<T> query, ItemSortBy sortType, SortOrder sortOrder) where T : BaseData, new()
    {
        TableQuery<T> sortedQuery = sortType switch
        {
            ItemSortBy.DateCreated => query.OrderBy(item => item.DateAdded),
            ItemSortBy.DatePlayed => query.OrderBy(item => item.DatePlayed),
            ItemSortBy.Name => query.OrderBy(item => item.Name),
            ItemSortBy.Random => query.OrderBy(item => Guid.NewGuid()),
            ItemSortBy.PlayCount => query.OrderBy(item => item.PlayCount),
            ItemSortBy.Album => query.OrderBy(item => item.Name),
            _ => query.OrderBy(item => item.Name)
        };
        
        if (sortOrder == SortOrder.Descending)
        {
            sortedQuery = sortType switch
            {
                ItemSortBy.DateCreated => query.OrderByDescending(item => item.DateAdded),
                ItemSortBy.DatePlayed => query.OrderByDescending(item => item.DatePlayed),
                ItemSortBy.Name => query.OrderByDescending(item => item.Name),
                ItemSortBy.Random => query.OrderBy(item => Guid.NewGuid()), // Random stays the same
                ItemSortBy.PlayCount => query.OrderByDescending(item => item.PlayCount),
                ItemSortBy.Album => query.OrderByDescending(item => item.Name),
                _ => query.OrderByDescending(item => item.Name)
            };
        }
        
        return sortedQuery;
    } 
}