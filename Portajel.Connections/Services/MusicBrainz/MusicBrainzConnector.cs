using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using MetaBrainz.MusicBrainz;

namespace Portajel.Connections.Services.MusicBrainz;

public class MusicBrainzConnector: IMediaServerConnector
{
    public Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; }
    public ConnectorFeeds? Feeds { get; set; }
    public Dictionary<string, IMediaDataConnector> DataConnectors { get; }
    public string Id { get; } = "MusicBrainzConnector";
    public string Name { get; } = "MusicBrainz";
    public string Description { get; } = "Connection to MusicBrainz";
    public string Image { get; }
    public ConnectorProperties Properties { get; set; }
    public SyncStatusInfo SyncStatus { get; set; }
    public AuthStatusInfo AuthStatus { get; set; }
    public async Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        return AuthStatusInfo.Ok();
    }

    public Task<bool> UpdateDb(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public List<Action<CancellationToken>> AuthenticateActions { get; set; }
    public List<Action<CancellationToken>> StartSyncActions { get; set; }
    public Task<bool> SetIsFavourite(Guid id, bool isFavourite, string serverUrl)
    {
        throw new NotImplementedException();
    }

    public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
        ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetAddress()
    {
        throw new NotImplementedException();
    }

    public string GetProfileImageUrl()
    {
        throw new NotImplementedException();
    }
}