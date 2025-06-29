﻿using SQLite;
using Newtonsoft.Json;
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using Portajel.Connections.Enum;
using Jellyfin.Sdk.Generated.Models;

namespace Portajel.Connections.Database
{
    public class AlbumData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string ArtistIdsJson { get; set; } = string.Empty;
        public string ArtistNames { get; set; } = string.Empty;
        public string SongIdsJson { get; set; } = string.Empty;
        public string GetSimilarJson { get; set; } = string.Empty;
        public override MediaTypes MediaType { get; set; } = MediaTypes.Album;
        public static AlbumData Empty { get; } = new();
        public Guid[] GetArtistIds()
        {
            Guid[] artistIds;
            try
            {
                artistIds = JsonConvert.DeserializeObject<Guid[]>(ArtistIdsJson);
            }
            catch (Exception)
            {
                // Handle cases where ArtistIdsJson might be null or invalid
                artistIds = null; 
            }
            return artistIds;
        }

        public Guid[] GetSongIds()
        {
            Guid[] songIds;
            try
            {
                songIds = JsonConvert.DeserializeObject<Guid[]>(SongIdsJson);
            }
            catch (Exception)
            {
                // Handle cases where SongIdsJson might be null or invalid
                songIds = [];
            }
            return songIds;
        }

        public Guid[] GetSimilarIds()
        {
            Guid[] similarIds;
            try
            {
                similarIds = JsonConvert.DeserializeObject<Guid[]>(GetSimilarJson);
            }
            catch (Exception)
            {
                // Handle cases where GetSimilarJson might be null or invalid
                similarIds = [];
            }
            return similarIds;
        }

        public static AlbumData Builder(BaseItemDto baseItem, string server, Guid[]? songIds = null, SongData[]? songDataItems = null)
        {
            if (baseItem.UserData == null)
            {
                throw new ArgumentException("Cannot create AlbumData without AlbumData UserData! Please fix server call flags!");
            }
            if (baseItem.Id == null)
            {
                throw new ArgumentException("Cannot create AlbumData without ID! Please fix server call flags!");
            }
            if (baseItem.ArtistItems == null)
            {
                throw new ArgumentException("Cannot create AlbumData without ArtistItems! Please fix server call flags!");
            }

            MusicItemImage musicItemImage = MusicItemImage.Builder(baseItem, server);
            AlbumData album = new()
            {
                ServerId = (Guid)baseItem.Id,
                Id = GuidHelper.GenerateNewGuidFromHash(baseItem.Id, server),
                Name = baseItem.Name ?? string.Empty,
                IsFavourite = baseItem.UserData.IsFavorite ?? false,
                PlayCount = baseItem.UserData.PlayCount ?? 0,
                DateAdded = baseItem.DateCreated,
                DatePlayed = baseItem.UserData.LastPlayedDate,
                ServerAddress = server,
                ImgSource = musicItemImage.Source,
                ImgBlurhash = musicItemImage.Blurhash,
                // Using Newtonsoft.Json.JsonConvert for serialization
                ArtistIdsJson = JsonConvert.SerializeObject(baseItem.ArtistItems.Select(idPair => idPair.Id).ToArray())
            };

            if (songIds != null)
            {
                // Using Newtonsoft.Json.JsonConvert for serialization
                album.SongIdsJson = JsonConvert.SerializeObject(songIds);
            }
            if (songDataItems != null && songDataItems.Length > 0)
            {
                // Using Newtonsoft.Json.JsonConvert for serialization
                album.SongIdsJson = JsonConvert.SerializeObject(songDataItems.Select(s => s.Id).ToArray());
                album.DatePlayed = songDataItems.OrderBy(s => s.DatePlayed).First().DatePlayed;
            }
            
            if (baseItem.ProviderIds != null && baseItem.ProviderIds.AdditionalData.TryGetValue("MusicBrainzAlbum", out var value))
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