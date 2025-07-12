using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Newtonsoft.Json; // Changed from System.Text.Json
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using SkiaSharp;
using System.IO;
using Portajel.Connections.Enum;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Database
{
    public class ArtistData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string? Description { get; set; }
        public string? LogoImgSource { get; set; }
        public string? BackgroundImgSource { get; set; }
        public string? BackgroundImgBlurhashSource { get; set; }
        public override MediaType MediaType { get; set; } = MediaType.Artist;
        public static ArtistData Empty { get; } = new();
    }
}