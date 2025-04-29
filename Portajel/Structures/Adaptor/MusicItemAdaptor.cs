using Jellyfin.Sdk.Generated.Models;
using Jellyfin.Sdk.Generated.Videos;
using Microsoft.Maui.Adapters;
using NetTopologySuite.Index.HPRtree;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Adaptor
{
    public class MusicItemAdaptor : VirtualListViewAdapterBase<object, BaseData>
    {
        private IDbConnector _database;
        public MusicItemAdaptor(IDbConnector database, int total) 
        {
            _database = database;
        }
        public MusicItemAdaptor(IDbConnector database)
        {
            _database = database;
        }
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            BaseData item = _database.Database.Table<AlbumData>().Skip(itemIndex).Take(1).First();
            var id = item.Id.ToString();
            return item;
        }
        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            int count = _database.GetDataConnectors()["Album"].GetTotalCount();
            return count;
        }
    }
}