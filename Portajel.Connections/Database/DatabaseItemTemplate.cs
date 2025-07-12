using System.Data;
using System.Reflection;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using SQLite;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Services.Database;

public class DatabaseItemTemplate: IDbItemConnector
{
    private readonly SQLiteConnection _database;
    private readonly Type _dataType;
    
    private static readonly Dictionary<MediaType, Type> MediaTypeToDataType = new()
    {
        { MediaType.Album, typeof(AlbumData) },
        { MediaType.Artist, typeof(ArtistData) },
        { MediaType.Song, typeof(SongData) },
        { MediaType.Playlist, typeof(PlaylistData) },
        { MediaType.Genre, typeof(GenreData) }
    };
    
    public MediaType MediaType { get; set; }

    public DatabaseItemTemplate(SQLiteConnection database, MediaType mediaType)
    {
        _database = database;
        MediaType = mediaType;
        _dataType = MediaTypeToDataType[mediaType];
    }
    
    public BaseData[] GetAll(
        int? limit = null, 
        int startIndex = 0,
        bool? getFavourite = null, 
        ItemSortBy setSortTypes = ItemSortBy.Album, 
        SortOrder setSortOrder = SortOrder.Descending, 
        Guid?[]? includeIds = null, 
        Guid?[]? excludeIds = null, 
        CancellationToken cancellationToken = default)
    {
        limit ??= 50;
        return (BaseData[])typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.GetTypedAll), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, limit, startIndex, getFavourite, setSortTypes, setSortOrder, includeIds, excludeIds })!;
    }

    public BaseData Get(Guid id, CancellationToken cancellationToken = default)
    {
        return (BaseData)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.GetTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, id })!;
    }

    public bool Contains(Guid id, CancellationToken cancellationToken = default)
    {
        return (bool)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.ContainsTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, id })!;
    }

    public int GetTotalCount(bool? getFavourite = null, CancellationToken cancellationToken = default)
    {
        return (int)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.GetTypedTotalCount), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, getFavourite })!;
    }

    public bool Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return (bool)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.DeleteTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, id })!;
    }

    public bool DeleteRange(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return (bool)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.DeleteRangeTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, ids })!;
    }

    public bool Insert(BaseData musicItem, CancellationToken cancellationToken = default)
    {
        return (bool)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.InsertTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, musicItem })!;
    }

    public bool InsertRange(BaseData[] musicItems, CancellationToken cancellationToken = default)
    {
        return (bool)typeof(DatabaseTypeConverters)
            .GetMethod(nameof(DatabaseTypeConverters.InsertRangeTyped), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(_dataType)
            .Invoke(null, new object[] { _database, musicItems })!;
    }
}