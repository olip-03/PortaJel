using System.Diagnostics;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;

namespace Portajel.Connections;

//  https://media.olisshittyserver.xyz/api-docs/swagger/index.html
public class ServerConnector : List<IMediaServerConnector>
{
    public ConnectorFeeds? Feeds { get; }
    public ConnectorProperties Properties { get; set; } = [];
    public EventHandler<IMediaServerConnector>? OnAdd { get; set; }
    public IMediaServerConnector? this[string name] => this.FirstOrDefault(s => s.GetAddress() == name);

    public ServerConnector()
    {
        Feeds  = new ServerConnectorFeeds(this);
    }

    public new void Add(IMediaServerConnector server)
    {
        base.Add(server);
        OnAdd?.Invoke(this, server);
    }

    public void Remove(string address)
    {
        var srv = this.FirstOrDefault(s => s.GetAddress() == address);
        if (srv != null)
        {
            this.Remove(srv);
        }
    }
    public async Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        int failed = 0;
        var tasks = this.Select(server => Task.Run(() =>
            {
                try
                {
                    // Get AlbumData data 
                    var toAdd = server.AuthenticateAsync(cancellationToken);
                    toAdd.Wait(cancellationToken);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    Interlocked.Increment(ref failed);
                    throw;
                }
            }, cancellationToken))
            .ToList();
        Task t = Task.WhenAll(tasks);
        try
        {
            await t;
        }
        catch (Exception ex)
        {
            return AuthStatusInfo.Failed(ex.Message);
        }
        
        switch (t.Status)
        {
            case TaskStatus.RanToCompletion:
                Trace.WriteLine($"All connections successfully authenticated");
                break;
            case TaskStatus.Faulted:
                Trace.WriteLine($"{failed} AlbumData request attempts failed!");
                break;
        }
        
        return AuthStatusInfo.Ok();
    }
    public async Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
    {
        int failed = 0;
        List<Task> syncJobs = new();
        var tasks = this.Select(server => Task.Run(() =>
        {
            try
            {
                if (server.Properties.TryGetValue("LastSyncDate", out ConnectorPropertyValue? value))
                {
                    DateTime lastSyncDate = DateTime.Parse((string)value.Value);
                    if ((DateTime.Now - lastSyncDate).TotalDays >= 90) // 3 months ~= 90 days
                    {
                        server.UpdateDb();
                        return;
                    }
                }
                syncJobs.Add(server.StartSyncAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Interlocked.Increment(ref failed);
                throw;
            }
        }, cancellationToken))
            .ToList();
        Task t = Task.WhenAll(tasks);
        try
        {
            await t;
        }
        catch { }

        foreach (Task sj in syncJobs)
        {
            await sj;
        }

        return true;
    }
    public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
        ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Array.Empty<BaseData>());
    }
    public ServerConnectorSettings GetSettings()
    {
        throw new NotImplementedException();
    }
}