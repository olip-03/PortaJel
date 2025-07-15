using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces
{
    public interface IServerConnector
    {
        MediaServerList Servers { get; }
        public ConnectorFeeds? Feeds { get; } 
        ConnectorProperties Properties { get; }
        Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default);
        Task<bool> StartSyncAsync(CancellationToken cancellationToken = default);
        Task<BaseData[]> SearchAsync(
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
    }
}