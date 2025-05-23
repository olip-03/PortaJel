﻿using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using PortaJel.Droid.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Services
{
    public class DroidDbConnector: IDbConnector
    {
        private DroidServiceController _serviceConnection = null!;

        public SQLiteConnection Database =>
            (_serviceConnection.AppServiceConnection.Binder?.Database.Database) ?? throw new Exception("Cannot retrieve value without Binder.");

        public DbConnectors Connectors =>
            _serviceConnection.AppServiceConnection.Binder?.Database != null ?
            _serviceConnection.AppServiceConnection.Binder.Database.Connectors :
            throw new Exception("Cannot retrieve value without Binder.");
        public BaseData[] Search(
            string searchTerm = "",
            int limit = 50, 
            int startIndex = 0, 
            ItemSortBy setSortTypes = ItemSortBy.Name, 
            SortOrder setSortOrder = SortOrder.Ascending, 
            CancellationToken cancellationToken = default) => 
            _serviceConnection.AppServiceConnection.Binder?.Database != null ?
            _serviceConnection.AppServiceConnection.Binder.Database.Search(
                searchTerm, 
                limit, 
                startIndex, 
                setSortTypes, 
                setSortOrder, 
                cancellationToken) :
            throw new Exception("Cannot retrieve value without Binder.");

        BaseData[] IDbConnector.Search(string searchTerm, int limit, int startIndex, ItemSortBy setSortTypes, SortOrder setSortOrder, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public DroidDbConnector(DroidServiceController serviceConnection)
        {
            _serviceConnection = serviceConnection;
        }
    }
}
