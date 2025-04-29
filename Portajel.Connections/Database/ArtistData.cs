using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using SkiaSharp;
using System.IO;

namespace Portajel.Connections.Database
{
    public class ArtistData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string? Description { get; set; }
        public string? LogoImgSource { get; set; }
        public string? BackgroundImgSource { get; set; }
        public string? BackgroundImgBlurhashSource { get; set; }
        public bool IsPartial { get; set; } = true;
        public string AlbumIdsJson { get; set;} = string.Empty;
        public static ArtistData Empty { get; } = new();

        public Guid[] GetAlbumIds()
        {
            Guid[]? guids = JsonSerializer.Deserialize<Guid[]>(AlbumIdsJson);
            return guids == null ? [] : guids;
        }

        public Guid[] GetSimilarIds()
        {
            return [];
        }

        public static ArtistData Builder(BaseItemDto baseItem, string server)
        {
            if (baseItem.Id == null)
            {
                throw new ArgumentException("Cannot create ArtistData without ArtistData ServerId! Please fix server call flags!");
            }
            if (baseItem.UserData == null)
            {
                throw new ArgumentException("Cannot create ArtistData without ArtistData UserData! Please fix server call flags!");
            }

            MusicItemImage artistLogo = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Logo);
            MusicItemImage artistBackdrop = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Backdrop);
            MusicItemImage artistImg = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Primary);

            ArtistData toAdd = new();
            toAdd.ServerId = (Guid)baseItem.Id;
            toAdd.Id = GuidHelper.GenerateNewGuidFromHash(baseItem.Id, server);
            toAdd.ServerAddress = server;
            toAdd.Name = baseItem.Name == null ? string.Empty : baseItem.Name;
            toAdd.DateAdded = baseItem.DateCreated;
            toAdd.IsFavourite = baseItem.UserData.IsFavorite == null ? false : (bool)baseItem.UserData.IsFavorite;
            toAdd.Description = baseItem.Overview == null ? string.Empty : baseItem.Overview;
            toAdd.LogoImgSource = artistLogo.Source;
            toAdd.ImgSource = artistImg.Source;
            toAdd.ImgBlurhash = artistImg.Blurhash;
            toAdd.BackgroundImgSource = artistBackdrop.Source;
            toAdd.BackgroundImgBlurhashSource = artistBackdrop.Blurhash;
            return toAdd;
        }
    }
}
