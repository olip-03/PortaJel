using Portajel.Components.Library;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Adaptor;

public class ItemTemplateSelector : VirtualListViewItemTemplateSelector
{
	public ItemTemplateSelector() : base()
	{
		AlbumTemplate = new DataTemplate(typeof(AlbumViewCell));
        ArtistTemplate = new DataTemplate(typeof(ArtistViewCell));
        GenreTemplate = new DataTemplate(typeof(GenreViewCell));
        PlaylistTemplate = new DataTemplate(typeof(PlaylistViewCell));
        SongTemplate = new DataTemplate(typeof(SongViewCell));
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
			if (mediaType == MediaTypes.Album)
				return AlbumTemplate;
			if(mediaType == MediaTypes.Artist)
				return ArtistTemplate;
            if (mediaType == MediaTypes.Genre)
                return GenreTemplate;
            if (mediaType == MediaTypes.Playlist)
                return PlaylistTemplate;
            if (mediaType == MediaTypes.Song)
                return SongTemplate;
        }
		return AlbumTemplate;
	}
}
