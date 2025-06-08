using System.Text.Json;
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
    public IServerConnector ServerConnector { get; private init; }
    public ServerConnectorSettings(string json, IDbConnector database, string appDataDirectory)
    {
        ServerConnector = new ServerConnector();
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            // Return empty list
            return;
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            var serverProperties = JsonSerializer.Deserialize<List<Dictionary<string, ConnectorProperty>>>(json, options);
            if (serverProperties == null) return;
            ServerConnector = new ServerConnector { Servers = new MediaServerList() };
            foreach (var props in serverProperties)
            {
                if (props.TryGetValue("ConnectorType", out var typeProperty))
                {
                    IMediaServerConnector server = null;
                    // Convert the Value to string or int as needed
                    var connectorTypeValue = typeProperty.Value?.ToString();

                    // Option 1: If the enum values match the numeric values
                    if (int.TryParse(connectorTypeValue, out int connectorTypeInt))
                    {
                        switch (connectorTypeInt)
                        {
                            case 3: // Assuming 3 means JellyFin
                                server = new JellyfinServerConnector(database)
                                {
                                    Properties = props
                                };
                                break;
                        }
                    }
                    // Option 2: Or if you expect string values in some cases
                    else
                    {
                        switch (connectorTypeValue)
                        {
                            case "JellyFin":
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
    public ServerConnectorSettings(IServerConnector serverConnector, IMediaServerConnector[] servers)
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
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
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
        return JsonSerializer.Serialize(ServerConnector.Servers.Select(s => s.Properties), options);
    }
}