using Portajel.Components.Library;

namespace Portajel.Structures.Adaptor;

public class GridTemplateSelector : VirtualListViewItemTemplateSelector
{
    public GridTemplateSelector() : base()
    {
        AlbumTemplate = new DataTemplate(typeof(AlbumGridViewCell));
    }

    readonly DataTemplate AlbumTemplate;

    public override DataTemplate SelectTemplate(object? item, int sectionIndex, int itemIndex)
    {
        return AlbumTemplate;   
    }
}