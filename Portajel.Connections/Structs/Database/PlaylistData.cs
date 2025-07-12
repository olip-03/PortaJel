using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Newtonsoft.Json; 
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using Portajel.Connections.Enum;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Database
{
    public class PlaylistData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public int ChildCount { get; set; }
        public TimeSpan Duration { get; set; } = new();
        public string Path { get; set; } = string.Empty;
        public override MediaType MediaType { get; set; } = MediaType.Playlist;
        public static PlaylistData Empty { get; set; } = new();
    }
}