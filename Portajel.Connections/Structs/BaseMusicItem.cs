using Portajel.Connections.Data;
using SkiaSharp;

namespace Portajel.Connections.Structs
{
    public abstract class BaseMusicItem
    {
        public virtual string ServerAddress { get; set; }
        public virtual Guid Id { get; set; }
        public virtual Guid ServerId { get; set; }
        public virtual string ImgSource { get; set; }
        public virtual string ImgBlurhash { get; set; }
        public SKBitmap ImgBlurhashBitmap { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsFavourite { get; set; }
        public virtual int PlayCount { get; set; }
        public virtual DateTimeOffset? DateAdded { get; set; }
        public virtual DateTimeOffset? DatePlayed { get; set; }
        public Album ToAlbum()
        {
            return (Album)this;
        }
        public Artist ToArtist()
        {
            return (Artist)this;
        }
        public Playlist ToPlaylist()
        {
            return (Playlist)this;
        }
        public Song ToSong()
        {
            return (Song)this;
        }
        public static bool IsNullOrEmpty(BaseMusicItem item)
        {
            if (item == null) return true;
            if (item is Album)
            {
                if (item == Album.Empty) return true;
            }
            else if (item is Song)
            {
                if (item == Song.Empty) return true;

            }
            else if (item is Artist)
            {
                if (item == Artist.Empty) return true;

            }
            else if (item is Playlist)
            {
                if (item == Playlist.Empty) return true;
            }
            return false;
        }
    }
}
