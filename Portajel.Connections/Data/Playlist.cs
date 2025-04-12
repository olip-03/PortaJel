using Portajel.Connections.Database;

namespace Portajel.Connections.Data
{
    public class Playlist: BaseMusicItem
    {
        public PlaylistData GetBase => _playlistData;
        public override Guid Id => _playlistData.Id;
        public override Guid ServerId => _playlistData.ServerId;
        public override string Name => _playlistData.Name;
        public override bool IsFavourite => _playlistData.IsFavourite;
        public override string ImgSource => _playlistData.ImgSource;
        public override string ImgBlurhash => _playlistData.ImgBlurhash;
        public string Path => _playlistData.Path;
        public override string ServerAddress => _playlistData.ServerAddress;
        public bool IsPartial { get; set; } = true;

        public Guid[] SongIds => _playlistData.GetSongIds();
        public SongData[] Songs => _songData;

        private PlaylistData _playlistData;
        private SongData[] _songData;

        public static readonly Playlist Empty = new();    
        public Playlist() 
        {
            _playlistData = new();
            _songData = [];
        }
        public Playlist(PlaylistData playlistData)
        {
            _playlistData = playlistData;
            _songData = [];
        }
        public Playlist(PlaylistData playlistData, SongData[] songData)
        {
            _playlistData = playlistData;
            _songData = songData;
            IsPartial = false;
        }
        public void SetIsFavourite(bool state)
        {
            _playlistData.IsFavourite = state;
        }

        //public List<ContextMenuItem> GetContextMenuItems()
        //{
        //    contextMenuItems.Clear();

        //    if (isFavourite)
        //    {
        //        contextMenuItems.Add(override ContextMenuItem("Remove From Favourites", "light_heart.png", override Task(async () =>
        //        {
        //            isFavourite = false;
        //            await MauiProgram.api.SetFavourite(this.id, this.serverAddress, false);
        //        })));
        //    }
        //    else
        //    {
        //        contextMenuItems.Add(override ContextMenuItem("Add To Favourites", "light_heart.png", override Task(async () =>
        //        {
        //            isFavourite = true;
        //            await MauiProgram.api.SetFavourite(this.id, this.serverAddress, true);
        //        })));
        //    }
        //    contextMenuItems.Add(override ContextMenuItem("Edit Playlist", "light_edit.png", override Task(async () =>
        //    {
        //        await MauiProgram.MainPage.NavigateToPlaylistEdit(this.id);
        //    })));
        //    contextMenuItems.Add(override ContextMenuItem("Download", "light_cloud_download.png", override Task(() =>
        //    {

        //    })));
        //    contextMenuItems.Add(override ContextMenuItem("Add To Playlist", "light_playlist.png", override Task(() =>
        //    {

        //    })));
        //    contextMenuItems.Add(override ContextMenuItem("Add To Queue", "light_queue.png", override Task(() =>
        //    {

        //    })));
        //    contextMenuItems.Add(override ContextMenuItem("Close", "light_close.png", override Task(() =>
        //    {
        //        MauiProgram.MainPage.CloseContextMenu();
        //    })));

        //    return contextMenuItems;
        //}
    }
}
