using Portajel.Connections.Database;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Data
{
    /// <summary>
    /// Represents 'songs' and all relevent information to display, and play them 
    /// </summary>
    public class Song : BaseMusicItem
    {
        public SongData GetBase => _songData;
        public override Guid Id => _songData.Id;
        public override Guid ServerId => _songData.ServerId;
        public Guid AlbumId => _songData.AlbumId;
        public override string Name => _songData.Name;
        public override bool IsFavourite => _songData.IsFavourite;
        public override int PlayCount => _songData.PlayCount;
        public override DateTimeOffset? DateAdded => _songData.DateAdded;
        public override DateTimeOffset? DatePlayed => _songData.DatePlayed;
        public override string ServerAddress => _songData.ServerAddress;
        public string? PlaylistId => _songData.PlaylistId;
        public override string ImgSource => _songData.ImgSource;
        public override string ImgBlurhash => _songData.ImgBlurhash;
        public ArtistData[] Artists => _artistData;
        public Guid[] ArtistIds => _songData.GetArtistIds();
        public string ArtistNames => _songData.ArtistNames;
        public AlbumData Album => _albumData;
        public int IndexNumber => _songData.IndexNumber;
        public string StreamUrl => _songData.StreamUrl;
        public int DiskNumber =>  _songData.DiskNumber;
        public TimeSpan Duration => _songData.Duration;
        public string FileLocation => _songData.FileLocation;
        public bool IsDownloaded => _songData.IsDownloaded;
        public bool IsPartial { get; private set; } = false;

        private SongData _songData = new();
        private AlbumData _albumData = new();
        private ArtistData[] _artistData = [];

        #region Constructors
        public static readonly Song Empty = new();
        public Song()
        {
            IsPartial = true;
        }
        public Song(SongData songData, AlbumData? albumData = null, ArtistData[]? artistData = null)
        {
            _songData = songData;
            _albumData = albumData == null ? new() : albumData;
            _artistData = artistData == null ? [] : artistData;

            if(_albumData == null || _artistData.Length  == 0)
            {
                IsPartial = true;
            }
        }
        #endregion

        #region Methods
        public void SetIsFavourite(bool state)
        {
            _songData.IsFavourite = state;

        }
        public void SetPlaylistId(string playlistId)
        {
            _songData.PlaylistId = playlistId;
        }
        #endregion
    }
}
