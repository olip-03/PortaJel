using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using MetaBrainz.MusicBrainz;
using Portajel.Connections.Services.Sync;

namespace Portajel.Connections.Services.MusicBrainz;

public class MusicBrainzConnector: IMediaServerConnector
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Image { get; }
    public ConnectorProperties Properties { get; set; }
    public SyncStatusInfo SyncStatus { get; set; }
    public AuthStatusInfo AuthStatus { get; set; }
    public Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; }
    public ConnectorFeeds? Feeds { get; set; }
    public Dictionary<string, IMediaDataConnector> DataConnectors { get; }
    public MusicBrainzConnector()
    {
        Id = "MusicBrainzConnector";
        Name = "MusicBrainz";
        Description = "Connection to MusicBrainz";
        Image = "";
        Properties = new ConnectorProperties();
        SyncStatus = new SyncStatusInfo();
        AuthStatus = new AuthStatusInfo();
    }
    
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