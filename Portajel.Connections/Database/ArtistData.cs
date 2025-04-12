﻿using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Data;
using PortaJel_Blazor.Classes;

namespace Portajel.Connections.Database
{
    public class ArtistData
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public Guid ServerId { get; set; }
        public Guid Id { get; set; }
        public string ServerAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? DateAdded { get; set; }
        public bool IsFavourite { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public string LogoImgSource { get; set; } = string.Empty;
        public string BackgroundImgSource { get; set; } = string.Empty;
        public string BackgroundImgBlurhash { get; set; } = string.Empty;
        public string ImgSource { get; set; } = string.Empty;
        public string ImgBlurhash { get; set; } = string.Empty;
        public string AlbumIdsJson { get; set;} = string.Empty;
        public bool IsPartial { get; set; } = true;
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
                throw new ArgumentException("Cannot create Artist without Artist ServerId! Please fix server call flags!");
            }
            if (baseItem.UserData == null)
            {
                throw new ArgumentException("Cannot create Artist without Artist UserData! Please fix server call flags!");
            }

            MusicItemImage artistLogo = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Logo);
            MusicItemImage artistBackdrop = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Backdrop);
            MusicItemImage artistImg = MusicItemImage.Builder(baseItem, server, ImageBuilderImageType.Primary);

            ArtistData toAdd = new();
            toAdd.ServerId = (Guid)baseItem.Id;
            toAdd.Id = GuidHelper.GenerateNewGuidFromHash(toAdd.Id, server);
            toAdd.ServerAddress = server;
            toAdd.Name = baseItem.Name == null ? string.Empty : baseItem.Name;
            toAdd.DateAdded = baseItem.DateCreated;
            toAdd.IsFavourite = baseItem.UserData.IsFavorite == null ? false : (bool)baseItem.UserData.IsFavorite;
            toAdd.Description = baseItem.Overview == null ? string.Empty : baseItem.Overview;
            toAdd.LogoImgSource = artistLogo.Source;
            toAdd.ImgSource = artistImg.Source;
            toAdd.ImgBlurhash = artistImg.Blurhash;
            toAdd.BackgroundImgSource = artistBackdrop.Source;
            toAdd.BackgroundImgBlurhash = artistBackdrop.Blurhash;

            return toAdd;
        }
    }
}
