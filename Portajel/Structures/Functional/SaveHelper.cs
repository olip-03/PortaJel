using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;
using System.Diagnostics;
using Newtonsoft.Json;
using Portajel.Connections.Definitions;

namespace Portajel.Structures.Functional
{
    public static class SaveHelper
    {
        private static string model = DeviceInfo.Current.Model;
        private static string manufacturer = DeviceInfo.Current.Manufacturer;
        private static string deviceName = DeviceInfo.Current.Name;

        public static async Task<IServerConnector> LoadData(IDbConnector database, string appDataDirectory)
        {
            Task<string?> r = SecureStorage.Default.GetAsync(GuidHelper.GetDeviceHash(model, manufacturer, deviceName));
            r.Wait();
            string? json = r.Result;
            if (json == null)
            {
                return new ServerConnector();
            }

            try
            {
                ServerConnectorSettings? settings = JsonConvert.DeserializeObject<ServerConnectorSettings>(json);
                MediaServerList servers = new();
                foreach (var server in settings.Servers)
                {
                    if (ConnectorDefinitions.ServerConnectorDefinitions.ContainsKey(server.Id))
                    {
                        IMediaServerConnector newServer =
                            ConnectorDefinitions.ServerConnectorDefinitions[server.Id].Factory.Invoke(database, server.Properties);
                        foreach (var savedFeed in server.MediaFeeds)
                        {
                            if (newServer.Feeds == null)
                            {
                                continue;
                            }

                            if (!newServer.Feeds.TryGetValue(savedFeed.Id, out var existingFeed)) continue;
                            existingFeed.Id = savedFeed.Id;
                            existingFeed.Name = savedFeed.Name;
                            existingFeed.Description = savedFeed.Description;
                            existingFeed.ServerUrl = savedFeed.ServerUrl;
                            existingFeed.IsEnabled = savedFeed.IsEnabled;
                            existingFeed.Properties = savedFeed.Properties;
                            existingFeed.ViewStyle = savedFeed.ViewStyle;
                            existingFeed.ServerUrl = savedFeed.ServerUrl;
                        }
                        servers.Add(newServer);
                    }
                }

                return new ServerConnector()
                {
                    Servers = servers,
                };
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new ServerConnector();
            }
        }

        public static async Task<bool> SaveData(IServerConnector server)
        {
            try
            {
                var json = ServerToJson(server);
                await SecureStorage.Default.SetAsync(GuidHelper.GetDeviceHash(model, manufacturer, deviceName), json);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"SaveData(): {e.Message}");
                return false;
            }

            return true;
        }

        private static string ServerToJson(IServerConnector server)
        {
            var settings = new ServerConnectorSettings()
            {
                Servers = server.Servers.Select(s => new ServerSettings(s)).ToList(),
            };
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            return json;
        }
    }
}