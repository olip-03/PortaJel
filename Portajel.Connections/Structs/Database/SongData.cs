using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Newtonsoft.Json; // Changed from System.Text.Json
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using Portajel.Connections.Enum;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Database
{
    public class SongData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string? PlaylistId { get; set; }
        public TimeSpan Duration { get; set; } = new();
        public int IndexNumber { get; set; } = 0;
        public int DiskNumber { get; set; } = 0;    
        public bool IsDownloaded { get; set; } = false;
        public string? ArtistNames { get; set; }
        public string ArtistIdsJson { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
        public override MediaType MediaType { get; set; } = MediaType.Song;
        public static SongData Empty { get; set; } = new();
    }
}