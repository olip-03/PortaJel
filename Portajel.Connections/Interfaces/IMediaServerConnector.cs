﻿using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces
{
    public interface IMediaServerConnector
    {
        Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; }
        public IFeedConnector? Feeds { get; } 
        Dictionary<string, IMediaDataConnector> DataConnectors { get; }
        public string Name { get; }
        public string Description { get; }
        public string Image { get; }
        public Dictionary<string, ConnectorProperty> Properties { get; set; }
        public SyncStatusInfo SyncStatus { get; set; }
        public AuthStatusInfo AuthStatus { get; set; }
        Task<AuthStatusInfo> AuthenticateAsync(
            CancellationToken cancellationToken = default);
        Task<bool> UpdateDb(
            CancellationToken cancellationToken = default);
        Task<bool> StartSyncAsync(
            CancellationToken cancellationToken = default);
        List<Action<CancellationToken>> AuthenticateActions { get; set; }
        List<Action<CancellationToken>> StartSyncActions { get; set; }
        Task<bool> SetIsFavourite(
            Guid id, 
            bool isFavourite,
            string serverUrl);
        public Task<BaseData[]> SearchAsync(
            string searchTerm = "", 
            int? limit = null, 
            int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name, 
            SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default);
        string GetUsername();
        string GetPassword();
        string GetAddress();
        string GetProfileImageUrl();
        UserCredentials GetUserCredentials();
        MediaServerConnection GetConnectionType();
    }
}
