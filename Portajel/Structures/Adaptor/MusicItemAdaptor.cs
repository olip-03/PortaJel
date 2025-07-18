﻿using Jellyfin.Sdk.Generated.Models;
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
    public class MusicItemAdaptor : VirtualListViewAdapterBase<object, BaseData>
    {
        private IDbItemConnector _database;
        public MusicItemAdaptor(IDbItemConnector database, int total) 
        {
            _database = database;
        }
        public MusicItemAdaptor(IDbItemConnector database)
        {
            _database = database;
        }
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            var result = _database.GetAll(limit: 1, startIndex: itemIndex, setSortOrder: SortOrder.Descending, setSortTypes: ItemSortBy.Name).First();
            // result.Index = itemIndex;
            return result ?? AlbumData.Empty;
        }
        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            // If this happens to be too slow cache it and use a private int total
            return _database.GetTotalCount();
        }
    }
}