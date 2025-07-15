using Portajel.Components.Library;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Adaptor;

public class ListTemplateSelector : VirtualListViewItemTemplateSelector
{
	public ListTemplateSelector() : base()
	{
		AlbumTemplate = new DataTemplate(typeof(AlbumListViewCell));
        ArtistTemplate = new DataTemplate(typeof(ArtistListViewCell));
        GenreTemplate = new DataTemplate(typeof(GenreListViewCell));
        PlaylistTemplate = new DataTemplate(typeof(PlaylistListViewCell));
        SongTemplate = new DataTemplate(typeof(SongListViewCell));
    }

	readonly DataTemplate AlbumTemplate;
    readonly DataTemplate ArtistTemplate;
    readonly DataTemplate GenreTemplate;
    readonly DataTemplate PlaylistTemplate;
    readonly DataTemplate SongTemplate;

    public override DataTemplate SelectTemplate(object item, int sectionIndex, int itemIndex)
	{
		if (item is BaseData trackInfo)
		{
			var mediaType = trackInfo.MediaType;
			if (mediaType == MediaType.Album)
				return AlbumTemplate;
			if(mediaType == MediaType.Artist)
				return ArtistTemplate;
            if (mediaType == MediaType.Genre)
                return GenreTemplate;
            if (mediaType == MediaType.Playlist)
                return PlaylistTemplate;
            if (mediaType == MediaType.Song)
                return SongTemplate;
        }
		return AlbumTemplate;
	}
}
