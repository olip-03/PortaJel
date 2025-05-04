using Portajel.Components;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Adaptor;

public class ItemTemplateSelector : VirtualListViewItemTemplateSelector
{
	public ItemTemplateSelector() : base()
	{
		GenericTemplate = new DataTemplate(typeof(GenericViewCell));
	}

	readonly DataTemplate GenericTemplate;

	public override DataTemplate SelectTemplate(object item, int sectionIndex, int itemIndex)
	{
		if (item is BaseData trackInfo)
		{
			var genreId = trackInfo.MediaType;

			if (genreId == MediaTypes.Album)
				return GenericTemplate;
			//if (genreId == 1 || genreId == 3 || genreId == 4 || genreId == 5 || genreId == 13)
			//	return HeavyTemplate;
			//else if (genreId == 10 || genreId == 19 || genreId == 18 || genreId == 21 || genreId == 22)
			//	return FilmTemplate;
		}

		return GenericTemplate;
	}
}
