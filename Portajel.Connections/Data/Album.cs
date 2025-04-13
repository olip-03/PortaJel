using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;
using System.Runtime.CompilerServices;

namespace Portajel.Connections.Data
{
    public class Album: BaseMusicItem
    {
        private readonly AlbumData _albumData = new();
        public AlbumData GetBase => _albumData;
        public override Guid Id => _albumData.Id;
        public override Guid ServerId => _albumData.ServerId;
        public override string Name => _albumData.Name;
        public override bool IsFavourite => _albumData.IsFavourite;
        public override int PlayCount => _albumData.PlayCount;
        public override DateTimeOffset? DateAdded => _albumData.DateAdded;
        public override DateTimeOffset? DatePlayed => _albumData.DatePlayed;
        public override string ServerAddress => _albumData.ServerAddress;
        public override string ImgSource =>   _albumData.ImgSource;
        public override string ImgBlurhash => _albumData.ImgBlurhash;
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
            Album toReturn = new Album(album, songs, artists);
            toReturn.ImgBlurhashBitmap = Blurhasher.Decode(toReturn.ImgBlurhash, 64, 64);
            return toReturn;
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
