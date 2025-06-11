using Microsoft.Maui.Adapters;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Adaptor;

public class MediaFeedAdaptor: VirtualListViewAdapterBase<object, BaseData>
{
    private IMediaFeed _feed;
    public override BaseData GetItem(int sectionIndex, int itemIndex)
    {
        return _feed.GetFrom(itemIndex, 1).First();
    }

    public override int GetNumberOfItemsInSection(int sectionIndex)
    {
        return _feed.Total();
    }
}