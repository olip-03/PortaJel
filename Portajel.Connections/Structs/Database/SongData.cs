﻿using Jellyfin.Sdk.Generated.Models;
using SQLite;
using Newtonsoft.Json; // Changed from System.Text.Json
using Portajel.Connections.Structs;
using Portajel.Connections.Services;
using Portajel.Connections.Enum;

namespace Portajel.Connections.Database
{
    public class SongData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string? PlaylistId { get; set; }
        public Guid AlbumId { get; set; }
        public TimeSpan Duration { get; set; } = new();
        public int IndexNumber { get; set; } = 0;
        public int DiskNumber { get; set; } = 0;    
        public bool IsDownloaded { get; set; } = false;
        public string? ArtistNames { get; set; }
        public string ArtistIdsJson { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
        public bool IsPartial { get; set; } = true;
        public override MediaTypes MediaType { get; set; } = MediaTypes.Song;
        public static SongData Empty { get; set; } = new();

        public Guid[] GetArtistIds()
        {
            Guid[]? artistIds = null;
            try
            {
                // Using Newtonsoft.Json.JsonConvert for deserialization
                artistIds = JsonConvert.DeserializeObject<Guid[]>(ArtistIdsJson);
            }
            catch (Exception)
            {
                // Handle cases where ArtistIdsJson might be null or invalid
                // artistIds will remain null
            }

            if (artistIds == null) return [];
            return artistIds;
        }

        public static SongData Builder(BaseItemDto baseItem, string server)
        {
            SongData song = new();

            if (baseItem.UserData == null)
            {
                throw new ArgumentException("Cannot create SongData without AlbumData UserData! Please fix server call flags!");
            }
            if (baseItem.Id == null)
            {
                throw new ArgumentException("Cannot create SongData without ID! Please fix server call flags!");
            }
            // The original code commented out this check, so we'll keep it that way.
            // if (baseItem.ParentId == null)
            // {
            //     baseItem.ParentId = baseItem.Id;
            //     // throw new ArgumentException("Cannot create SongData without Parent ID! Please fix server call flags!");
            // }
            if (baseItem.ArtistItems == null)
            {
                throw new ArgumentException("Cannot create SongData without ArtistData Items! Please fix server call flags!");
            }
            if(baseItem.RunTimeTicks.HasValue)
            {   // TODO: Figure out why the fuck not all songs have a duration value..
                song.Duration = TimeSpan.FromTicks(baseItem.RunTimeTicks.Value);
            }
            MusicItemImage musicItemImage = MusicItemImage.Builder(baseItem, server);

            song.Id = GuidHelper.GenerateNewGuidFromHash(baseItem.Id, server);
            song.ServerId = (Guid)baseItem.Id;
            song.PlaylistId = baseItem.PlaylistItemId;
            // Ensure ParentId is treated as Guid, providing a default if null
            song.AlbumId = baseItem.ParentId.HasValue ? (Guid)baseItem.ParentId : Guid.Empty; 
            
            // Using Newtonsoft.Json.JsonConvert for serialization
            song.ArtistIdsJson = JsonConvert.SerializeObject(baseItem.ArtistItems.Select(item => item.Id).ToArray());
            
            song.Name = baseItem.Name == null ? string.Empty : baseItem.Name;
            song.IsFavourite = baseItem.UserData.IsFavorite == null ? false : (bool)baseItem.UserData.IsFavorite;
            // song.PlayCount = songData.PlayCount; TODO: Implement playcount idk
            song.DateAdded = baseItem.DateCreated;
            song.DatePlayed = baseItem.UserData.LastPlayedDate;
            song.IndexNumber = baseItem.IndexNumber == null ? 0 : (int)baseItem.IndexNumber;
            song.DiskNumber = baseItem.ParentIndexNumber == null ? 0 : (int)baseItem.ParentIndexNumber;
            song.ServerAddress = server;
            song.IsDownloaded = false; // TODO: Check if file exists idk 
            song.FileLocation = string.Empty; // TODO: Add file location
            song.StreamUrl = server + "/Audio/" + baseItem.Id + "/stream?static=true&audioCodec=adts&enableAutoStreamCopy=true&allowAudioStreamCopy=true&enableMpegtsM2TsMode=true&context=Static";
            song.ImgSource = musicItemImage.Source;
            song.ImgBlurhash = musicItemImage.Blurhash;

            string artistNames = string.Empty;
            for (int i = 0; i < baseItem.ArtistItems.Count; i++)
            {
                artistNames += baseItem.ArtistItems[i].Name;
                if (i < baseItem.ArtistItems.Count - 1)
                {
                    artistNames += ", ";
                }
            }
            song.ArtistNames = artistNames;

            return song;
        }
    }
}