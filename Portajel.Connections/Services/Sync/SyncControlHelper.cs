using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Services.Sync
{
    public static class SyncControlHelper
    {
        // public static async Task 
        public static async Task<(MediaType MediaType, int Total)> GetServerTotalCount(string name, IMediaDataConnector dataConnection)
        {
            return (dataConnection.MediaType, await dataConnection.GetTotalCountAsync());
        } 
    }
}
