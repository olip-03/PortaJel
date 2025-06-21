using Newtonsoft.Json;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Discogs;
using Portajel.Connections.Services.FS;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Services.Spotify;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;

namespace Portajel.Connections.Structs;

public class ServerConnectorSettings
{
    public ServerConnector ServerConnector { get; private init; }
    public ServerConnectorSettings(string json, IDbConnector database, string appDataDirectory)
    {
        ServerConnector = new ServerConnector();
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            return;
        }

        try
        {
            var serverProperties = JsonConvert.DeserializeObject<List<Dictionary<string, ConnectorProperty>>>(json);
            if (serverProperties == null) return;
        
            foreach (var props in serverProperties)
            {
                if (props.TryGetValue("ConnectorType", out var typeProperty))
                {
                    IMediaServerConnector server = null;
                    var connectorTypeValue = typeProperty.Value?.ToString();
                    if (int.TryParse(connectorTypeValue, out int connectorTypeInt))
                    {
                        switch (connectorTypeInt)
                        {
                            case 3: // JellyFin
                                server = new JellyfinServerConnector(database)
                                {
                                    Properties = props
                                };
                                break;
                        }
                    }

                    if (server != null)
                    {
                        server.Properties = props;
                        ServerConnector.Servers.Add(server);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
    
    public ServerConnectorSettings(ServerConnector serverConnector, IMediaServerConnector[] servers)
    {
        ServerConnector = serverConnector;
        if (ServerConnector.Servers.Count > 0) return;
        if (servers == null) return;
        foreach (var srv in servers)
        {
            ServerConnector.AddServer(srv);
        }
    }
    
    public string ToJson()
    {
        foreach (var server in ServerConnector.Servers)
        {
            if (!server.Properties.TryAdd("ConnectorType", new ConnectorProperty(
                    label: "ConnectorType",
                    description: "The Connection Type of this server.",
                    value: server.GetConnectionType(),
                    protectValue: false,
                    userVisible: false)
                ))
            {
                server.Properties["ConnectorType"].Value = server.GetConnectionType();
            }
        }
        return JsonConvert.SerializeObject(
            ServerConnector.Servers.Select(s => s.Properties), 
            Formatting.Indented
        );
    }
}