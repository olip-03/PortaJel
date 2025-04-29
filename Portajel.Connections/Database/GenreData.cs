using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Data;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Database;

public class GenreData: BaseData
{
    [PrimaryKey, NotNull, AutoIncrement] public override Guid Id { get; set; }
    public string AlbumIdsJson { get; set; } = string.Empty;
    public static GenreData Empty { get; } = new();
    public Guid[] GetAlbumIds()
    {
        return [];
    }
}