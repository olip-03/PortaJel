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
    public int ChildCount { get; set; }
    public TimeSpan Duration { get; set; } = new();
    public override MediaType MediaType { get; set; } = MediaType.Genre;
    public static GenreData Empty { get; } = new();
}