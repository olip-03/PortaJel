using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Data;
using Portajel.Connections.Structs;
using Portajel.Connections.Enum;

namespace Portajel.Connections.Database;

public class GenreData: BaseData
{
    [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
    public string AlbumIdsJson { get; set; } = string.Empty;
    public override MediaTypes MediaType { get; set; } = MediaTypes.Genre;
    public static GenreData Empty { get; } = new();
    public Guid[] GetAlbumIds()
    {
        return [];
    }
}