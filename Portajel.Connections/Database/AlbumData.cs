using Jellyfin.Sdk.Generated.Models;
using SQLite;
using System.Text.Json;
using Portajel.Connections.Structs;
using Portajel.Connections.Services;

namespace Portajel.Connections.Database
{
    public class AlbumData : BaseData
    {
        [PrimaryKey, AutoIncrement] public override Guid? Id { get; set; }
        public string ArtistIdsJson { get; set; } = string.Empty;
        public string ArtistNames { get; set; } = string.Empty;
        public string SongIdsJson { get; set; } = string.Empty;
        public string GetSimilarJson { get; set; } = string.Empty;
        public int Index { get; set; } = 0;
        public static AlbumData Empty { get; } = new();
        public Guid[] GetArtistIds()
        {
            Guid[] artistIds;
            try
            {
                artistIds = JsonSerializer.Deserialize<Guid[]>(ArtistIdsJson);
            }
            catch (Exception)
            {
                artistIds = null;
            }
            return artistIds;
        }
        public Guid[] GetSongIds()
        {
            Guid[] songIds;
            try
            {
                songIds = JsonSerializer.Deserialize<Guid[]>(SongIdsJson);
            }
            catch (Exception)
            {
                songIds = [];
            }
            return songIds;
        }
        public Guid[] GetSimilarIds()
        {
            Guid[] similarIds;
            try
            {
                similarIds = JsonSerializer.Deserialize<Guid[]>(GetSimilarJson);
            }
            catch (Exception)
            {
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
            AlbumData album = new();
            album.ServerId = (Guid)baseItem.Id;
            album.Id = GuidHelper.GenerateNewGuidFromHash(baseItem.Id, server);
            album.Name = baseItem.Name == null ? string.Empty : baseItem.Name;
            album.IsFavourite = baseItem.UserData.IsFavorite == null ? false : (bool)baseItem.UserData.IsFavorite;
            // album.PlayCount = albumData.PlayCount; TODO: Implement playcount
            album.DateAdded = baseItem.DateCreated;
            album.DatePlayed = baseItem.UserData.LastPlayedDate;
            album.ServerAddress = server;
            album.ImgSource = musicItemImage.Source;
            album.ImgBlurhash = musicItemImage.Blurhash;
            album.ArtistIdsJson = JsonSerializer.Serialize(baseItem.ArtistItems.Select(idPair => idPair.Id).ToArray());
            if (songIds != null)
            {
                album.SongIdsJson = JsonSerializer.Serialize(songIds);
            }
            if (songDataItems != null && songDataItems.Length > 0)
            {
                album.SongIdsJson = JsonSerializer.Serialize(songDataItems.Select(s => s.Id).ToArray());
                album.DatePlayed = songDataItems.OrderBy(s => s.DatePlayed).First().DatePlayed;
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
