using Jellyfin.Sdk.Generated.Models;
using Microsoft.Maui.Adapters;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Adaptor
{ 
    public class MusicItemAdaptor(IDbItemConnector database, bool favs, bool descendingOrder) : VirtualListViewAdapterBase<object, BaseData>
    {
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            SortOrder sortOrder = descendingOrder ? SortOrder.Descending : SortOrder.Ascending;
            var result = database.GetAll(
                limit: 1, 
                getFavourite: favs,
                startIndex: itemIndex, 
                setSortOrder: sortOrder, 
                setSortTypes: ItemSortBy.Name).First();
            return result;
        }
        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            return database.GetTotalCount(favs);
        }
    }

    public class MemoryItemAdaptor() : VirtualListViewAdapterBase<object, BaseData>
    {
        public List<BaseData> Items = new();
        
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            return Items[itemIndex];
        }

        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            return Items.Count;
        }
        public void SetItems(IEnumerable<BaseData> items)
        {
            Items = items.ToList();
        }
    }
}