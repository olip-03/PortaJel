using Portajel.Connections.Data;
using Portajel.Connections.Database;
using SkiaSharp;
using SQLite;

namespace Portajel.Connections.Structs
{
    public abstract class BaseData
    {
        [PrimaryKey, NotNull, AutoIncrement] public virtual Guid Id { get; set; }
        public Guid ServerId { get; set; }
        [Indexed] public string Name { get; set; } = string.Empty;
        public bool IsFavourite { get; set; }
        public int PlayCount { get; set; }
        public DateTimeOffset? DateAdded { get; set; }
        public DateTimeOffset? DatePlayed { get; set; }
        public string ServerAddress { get; set; } = string.Empty;
        public string? ImgSource { get; set; }
        public string? ImgBlurhash { get; set; }
        public string? ImgBlurhashSource { get; set; }

        public AlbumData ToAlbum()
        {
            return (AlbumData)this;
        }
        public ArtistData ToArtist()
        {
            return (ArtistData)this;
        }
        public PlaylistData ToPlaylist()
        {
            return (PlaylistData)this;
        }
        public SongData ToSong()
        {
            return (SongData)this;
        }
        public static bool IsNullOrEmpty(BaseData item)
        {
            if (item == null) return true;
            if (item is AlbumData)
            {
                if (item == AlbumData.Empty) return true;
            }
            else if (item is SongData)
            {
                if (item == SongData.Empty) return true;

            }
            else if (item is ArtistData)
            {
                if (item == ArtistData.Empty) return true;

            }
            else if (item is PlaylistData)
            {
                if (item == PlaylistData.Empty) return true;
            }
            return false;
        }
    }
}
