using Jellyfin.Sdk.Generated.Models;
using Jellyfin.Sdk.Generated.Videos;
using Microsoft.Maui.Adapters;
using NetTopologySuite.Index.HPRtree;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Adaptor
{ 
    public class MusicItemAdaptor(IDbItemConnector database) : VirtualListViewAdapterBase<object, BaseData>
    {
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            var result = database.GetAll(
                limit: 1, 
                startIndex: itemIndex, 
                setSortOrder: SortOrder.Descending, 
                setSortTypes: ItemSortBy.Name).First();
            return result;
        }
        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            return database.GetTotalCount();
        }
    }
}