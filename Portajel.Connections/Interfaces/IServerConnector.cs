using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Interfaces
{
    public interface IServerConnector
    {
        List<IMediaServerConnector> Servers { get; }
        Dictionary<string, ConnectorProperty> Properties { get; }
        Task<AuthResponse> AuthenticateAsync(CancellationToken cancellationToken = default);
        Task<bool> StartSyncAsync(CancellationToken cancellationToken = default);
        Task<BaseMusicItem[]> SearchAsync(
            string searchTerm = "",
            int? limit = null,
            int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name,
            SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default);
        public ServerConnectorSettings GetSettings();
        List<Action<IMediaServerConnector>> AddServerActions { get; set; }
        public void AddServer(IMediaServerConnector server);
        public void RemoveServer(IMediaServerConnector server);
        public void RemoveServer(string address);
        public IMediaServerConnector[] GetServers();
    }
}
