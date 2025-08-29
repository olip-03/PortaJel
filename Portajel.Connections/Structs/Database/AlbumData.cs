using SQLite;
using Newtonsoft.Json;
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using Portajel.Connections.Enum;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Services.Jellyfin.Dto;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Database
{
    public class AlbumData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string ArtistIdsJson { get; set; }
        public string GenresJson { get; set; }
        public string ArtistNames { get; set; } = string.Empty;
        public override MediaType MediaType { get; set; } = MediaType.Album;
        public static AlbumData Empty { get; } = new();
        public DateTimeOffset? PremiereDate { get; set; }
        public int? PremiereYear { get; set; }

        public Guid[] GetArtistIds()
        {
            return JsonConvert.DeserializeObject<Guid[]>(ArtistIdsJson);
        }
        public Dictionary<string, string> GetGenres()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(GenresJson);
        }
    }
}