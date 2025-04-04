﻿using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Database;

namespace Portajel.Connections.Data
{
    public class Album: BaseMusicItem
    {
        private readonly AlbumData _albumData = new();
        public AlbumData GetBase => _albumData;
        public new Guid LocalId => _albumData.LocalId;
        public new Guid Id => _albumData.Id;
        public new string Name => _albumData.Name;
        public new bool IsFavourite => _albumData.IsFavourite;
        public new int PlayCount => _albumData.PlayCount;
        public new DateTimeOffset? DateAdded => _albumData.DateAdded;
        public new DateTimeOffset? DatePlayed => _albumData.DatePlayed;
        public new string ServerAddress => _albumData.ServerAddress;
        public new string ImgSource =>   _albumData.ImgSource;
        public new string ImgBlurhash => _albumData.ImgBlurhash;
        public ArtistData[] Artists { get; }
        public string ArtistNames => _albumData.ArtistNames;
        public Guid[] ArtistIds => _albumData.GetArtistIds();
        public SongData[] Songs { get; }
        public Guid[] SimilarIds => _albumData.GetSimilarIds();
        public bool IsPartial { get; private set; } = true;
        
        public AlbumSortMethod sortMethod { get; set; } = AlbumSortMethod.name;
        public enum AlbumSortMethod
        {
            name,
            artist,
            id
        }
        public Album(AlbumData albumData = null, SongData[] songData = null, ArtistData[] artistData = null)
        {
            Artists = artistData ?? [];
            Songs = songData ?? [];
            _albumData = albumData ?? new();
        }

        public static readonly Album Empty = new();

        public static Album Builder(BaseItemDto albumData, string server, BaseItemDto[]? songData = null, BaseItemDto[]? artistData = null)
        {
            AlbumData album = AlbumData.Builder(albumData, server);
            SongData[] songs = [];
            ArtistData[] artists = [];
            if(songData != null)
            {
                songs = songData.Select(data => SongData.Builder(data, server)).ToArray();
            }
            if(artistData  != null)
            {
                artists = artistData.Select(data => ArtistData.Builder(data, server)).ToArray();
            }
            return new Album(album, songs, artists);
        }
        public Song[] GetSongs()
        {
            return Songs == null ? [] : Songs.Select(song => new Song(song, _albumData, Artists)).ToArray();
        }
        public void SetIsFavourite(bool state)
        {
            _albumData.IsFavourite = state;
        }

    }
}
