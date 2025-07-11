using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Data;
using Portajel.Connections.Structs;
using Portajel.Connections.Enum;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Database;

public class GenreData: BaseData
{
    [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
    public string AlbumIdsJson { get; set; } = string.Empty;
    public override MediaType MediaType { get; set; } = MediaType.Genre;
    public static GenreData Empty { get; } = new();
    public Guid[] GetAlbumIds()
    {
        return [];
    }

    public static GenreData Builder(BaseItemDto baseItem, string server, Guid[]? songIds = null, SongData[]? songDataItems = null)
    {
        // Todo: Implement
        return null;
    } 
}