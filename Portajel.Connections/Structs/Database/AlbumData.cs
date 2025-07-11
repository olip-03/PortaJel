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
        public string ArtistNames { get; set; } = string.Empty;
        public override MediaType MediaType { get; set; } = MediaType.Album;
        public static AlbumData Empty { get; } = new();
        public static AlbumData Builder(JfBaseItemDto baseItem, string server, Guid[]? songIds = null, SongData[]? songDataItems = null)
        {
            if (baseItem.UserData == null)
                throw new ArgumentException("Cannot create AlbumData without AlbumData UserData! Please fix server call flags!");
            if (baseItem.Id == null)
                throw new ArgumentException("Cannot create AlbumData without ID! Please fix server call flags!");
            if (baseItem.ArtistItems == null)
                throw new ArgumentException("Cannot create AlbumData without ArtistItems! Please fix server call flags!");

            // MusicItemImage musicItemImage = MusicItemImage.Builder(baseItem, server);
            AlbumData album = new()
            {
                ServerId = baseItem.Id,
                Id = GuidHelper.GenerateNewGuidFromHash(baseItem.Id, server),
                Name = baseItem.Name ?? string.Empty,
                IsFavourite = baseItem.UserData.IsFavorite,
                PlayCount = baseItem.UserData.PlayCount,
                DateAdded = baseItem.DateCreated,
                DatePlayed = baseItem.UserData.LastPlayedDate,
                ServerAddress = server,
                // TODO: Images, again. Base them off the data in ImageTags plz & thx
                // ImgSource = musicItemImage.Source,
                // ImgBlurhash = musicItemImage.Blurhash,
                ArtistIdsJson =
                    JsonConvert.SerializeObject(baseItem.ArtistItems.Select(idPair => idPair.Id.ToString() ?? "")),
            };
            
            if (songDataItems != null && songDataItems.Length > 0)
            {
                album.ArtistIdsJson = JsonConvert.SerializeObject(songDataItems.Select(idPair => idPair.GetArtistIds().ToString() ?? ""));
                album.DatePlayed = songDataItems.OrderBy(s => s.DatePlayed).First().DatePlayed;
            }
            
            if (baseItem.ProviderIds != null && baseItem.ProviderIds.TryGetValue("MusicBrainzAlbum", out var value))
            {
                Guid.TryParse(value.ToString(), out var albumId);
                album.GlobalId = albumId;
            }
            else
            {
                album.GlobalId = album.ServerId;
            }

            string artistNames = string.Empty;
            for (int i = 0; i < baseItem.ArtistItems.Count; i++)
            {
                artistNames += baseItem.ArtistItems[i].Name;
                if (i < baseItem.ArtistItems.Count - 1)
                {
                    artistNames += ", ";
                }
            }
            album.ArtistNames = artistNames;

            return album;
        }
    }
}