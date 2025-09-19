using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Enum;
using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Portajel.Connections.Services.Jellyfin.Dto;
using Portajel.Connections.Structs;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Services.Jellyfin
{
    // Allocation nightmare. I'm actually not going to bother
    // much here.
    public class JellyfinServerConnector : IMediaServerConnector
    {
        private readonly HttpClient _httpClient = new();
        private readonly IDbConnector _database;

        private JfAuthRootInfo _authInfo;
        // public UserDto? _userDto;
        // private SessionInfoDto? _sessionInfo;
        // public JellyfinSdkSettings? _sdkClientSettings;
        // public JellyfinApiClient? _jellyfinApiClient; // TODO: Set to private, for testing
        
        public IMediaDataConnector AlbumData { get; set; } = null!;
        public IMediaDataConnector ArtistData { get; set; } = null!;
        public IMediaDataConnector SongData { get; set; } = null!;
        public IMediaDataConnector PlaylistData { get; set; } = null!;
        public IMediaDataConnector Genre { get; set; } = null!;
        public ConnectorFeeds? Feeds { get; set; } = null!;
        public Dictionary<string, IMediaDataConnector> DataConnectors => new()
        {
            { "Album", AlbumData },
            { "Artist", ArtistData },
            { "Song", SongData },
            { "Playlist", PlaylistData },
            { "Genre", Genre }
        };
        public Dictionary<MediaCapabilities, bool> SupportedReturnTypes { get; set; } = new()
        {
            { MediaCapabilities.Album, true },
            { MediaCapabilities.Artist, true },
            { MediaCapabilities.Song, true },
            { MediaCapabilities.Playlist, true },
            { MediaCapabilities.Genre, true }
        };

        public string Id { get; } = "JellyfinServerConnector";
        public string Name { get; } = "JellyFin";
        public string Description { get; } = "Enables connections to the Jellyfin Media Server.";
        public string Image { get; } = "icon_jellyfin.png";
        public ConnectorProperties Properties { get; set; } = new();
        public SyncStatusInfo SyncStatus { get; set; } = new();
        public List<Action<CancellationToken>> AuthenticateActions { get; set; } = new();
        public List<Action<CancellationToken>> StartSyncActions { get; set; } = new();
        public JellyfinServerConnector()
        {
            
        }
        public JellyfinServerConnector(IDbConnector database, ConnectorProperties properties)
        {
            _database = database;
            Properties = properties;
            Feeds = new JellyfinConnectorFeeds(_database, Properties["URL"].Value.ToString());
        }
        public JellyfinServerConnector(
            IDbConnector database,
            string url = "",
            string username = "",
            string password = "",
            string appName = "",
            string appVerison = "",
            string deviceName = "",
            string deviceId = "",
            string appDataPath = "")
        {
            _database = database;
            Properties =
                new()
                {
                    {
                        "AppName", new ConnectorPropertyValue(
                            label: "App Name",
                            description: "The name of the Jellyfin Client Application.",
                            value: appName,
                            protectValue: false,
                            userVisible: true
                            )
                    },
                    {
                        "URL", new ConnectorPropertyValue(
                            label: "Url",
                            description: "The URL of the Jellyfin Server",
                            value: url,
                            protectValue: false,
                            userVisible: true)
                    },
                    {
                        "Username", new ConnectorPropertyValue(
                            label: "Username",
                            description: "Username for data at Url.",
                            value: username,
                            protectValue: false,
                            userVisible: true)
                    },
                    {
                        "Password", new ConnectorPropertyValue(
                            label: "Password",
                            description: "User password for data at Url.",
                            value: password,
                            protectValue: true,
                            userVisible: true)
                    },

                    {
                        "AppVersion", new ConnectorPropertyValue(
                            label: "App Version",
                            description: "The version of the Jellyfin Client Application.",
                            value: appVerison,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "DeviceName", new ConnectorPropertyValue(
                            label: "Device Name",
                            description: "The name of the device running this Jellyfin Client Application.",
                            value: deviceName,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "DeviceID", new ConnectorPropertyValue(
                            label: "Device Name",
                            description: "The name of the device running this Jellyfin Client Application.",
                            value: deviceId,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "LastSync", new ConnectorPropertyValue(
                            label: "Last Sync",
                            description: "The last time a full sync ran for this data.",
                            value: url,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "AppDataPath", new ConnectorPropertyValue(
                            label: "App Data Path",
                            description: "Application Data Path for storing files.",
                            value: appDataPath,
                            protectValue: false,
                            userVisible: false)
                    },
                };
        }
        public AuthStatusInfo AuthStatus { get; set; } = new AuthStatusInfo();
        public async Task<AuthStatusInfo> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            AuthStatus = AuthStatusInfo.CreateInProgress();
            string? baseUrl = Properties["URL"].Value.ToString();
            Guid userId = new();
            if (baseUrl == null)
            {
                return AuthStatusInfo.CreateFailed("No base url");
            }

            try
            {
                string authHeader =
                    $"Client=\"Portajel\", " +
                    $"Device=\"{Properties["DeviceName"].Value}\", " +
                    $"DeviceId=\"{Properties["DeviceID"].Value}\", " +
                    $"Version=\"{Properties["AppVersion"].Value}\"";
                string appName = (string)Properties["AppName"].Value;
                string appVersion = (string)Properties["AppVersion"].Value;

                var defaultHeaders = new Dictionary<string, string>
                {
                    { "Username", "local" },
                    { "Pw", "test1234" }
                };
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{baseUrl}/Users/authenticatebyname"),
                    Headers = { 
                        { "Accept", "application/json" },
                        { "User-Agent", $"{appName}/{appVersion}" },
                        { "Authorization", $"MediaBrowser {authHeader}" }
                    },
                    Content = new StringContent(JsonConvert.SerializeObject(defaultHeaders), Encoding.UTF8, "application/json")
                };
                var loginResponse = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
                string jsonContent = await loginResponse.Content.ReadAsStringAsync(cancellationToken);
                JfAuthRootInfo? result = JsonConvert.DeserializeObject<JfAuthRootInfo>(jsonContent);
                if (result != null)
                {
                    _authInfo = result;
                    
                    var userView = await GetUserViewId(authHeader, baseUrl, _authInfo);
                    if (userView != null)
                    {
                        AlbumData = new JellyfinItemConnectorTemplate(MediaType.Album, _httpClient,
                            _authInfo, baseUrl, userView);
                        ArtistData = new JellyfinItemConnectorTemplate(MediaType.Artist, _httpClient,
                            _authInfo, baseUrl, userView);
                        SongData = new JellyfinItemConnectorTemplate(MediaType.Song, _httpClient, _authInfo,
                            baseUrl,userView);
                        PlaylistData = new JellyfinItemConnectorTemplate(MediaType.Playlist, _httpClient,
                            _authInfo,baseUrl, userView);
                        Genre = new JellyfinItemConnectorTemplate(MediaType.Genre, _httpClient, _authInfo,
                            baseUrl, userView);
                        Feeds ??= new JellyfinConnectorFeeds(_database, baseUrl);
                    }
                    
                    return AuthStatusInfo.Ok();
                }
                return AuthStatusInfo.Failed("Failed to deserialize server response.");
            }
            catch (ApiException apiEx)
            {
                // More detailed API exception handling
                Trace.WriteLine($"Error: {apiEx.Message}");
                Trace.WriteLine($"Status code: {apiEx.ResponseStatusCode}");
                Trace.WriteLine($"Source: {apiEx.Source}");

                AuthStatus = new AuthStatusInfo()
                {
                    State = AuthState.Failed,
                    Message = $"API Error: {apiEx.Message} (Status: {apiEx.ResponseStatusCode})"
                };
                return AuthStatus;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
                Trace.WriteLine($"Source: {ex.StackTrace}");
                AuthStatus = new AuthStatusInfo()
                {
                    State = AuthState.Failed,
                    Message = $"{ex.Message}"
                };
                return AuthStatus;
            }
            finally
            {
                var actions = AuthenticateActions.Select(a => Task.Run(() =>
                {
                    a.Invoke(cancellationToken);
                }));
                _ = Task.WhenAll(actions);
            }
        }
        public async Task<bool> UpdateDb(CancellationToken cancellationToken = default)
        {
            await UpdateSyncStatus(cancellationToken);
            var tasks = DataConnectors.Values.Select(data => Task.Run(async () =>
            {
                int retrieve = 50;
                int checkCount = 0;

                data.SyncStatusInfo.TaskStatus = TaskStatus.Running;
                while (data.SyncStatusInfo.TaskStatus is TaskStatus.Running)
                {
                    try
                    {
                        // get items from server
                        var items = await data.GetAllAsync(
                            limit: retrieve,
                            startIndex: data.SyncStatusInfo.ServerItemCount,
                            setSortOrder: SortOrder.Descending,
                            setSortTypes: ItemSortBy.DateCreated,
                            cancellationToken: cancellationToken
                        );

                        int newTotal = data.SyncStatusInfo.ServerItemCount + items.Length;
                        double newPercent = ((double)newTotal / data.SyncStatusInfo.ServerItemTotal) * 100;
                        data.SetSyncStatusInfo(serverItemCount: newTotal, percentage: (int)newPercent);

                        // Check if these items are in the database
                        foreach (var item in items)
                        {
                            // If our DB has this item
                            if (GetDb(data).Value.Contains((Guid)item.Id))
                            {
                                checkCount++;
                            }
                            else 
                            {
                                GetDb(data).Value.Insert(item);
                            }
                        }
                        data.SetSyncStatusInfo(serverItemCount: GetDb(data).Value.GetTotalCount());
                        if(items.Length < retrieve)
                        {
                            data.SetSyncStatusInfo(status: TaskStatus.RanToCompletion, percentage: (int)100);
                            break;
                        }
                        if (checkCount >= retrieve)
                        {
                            data.SetSyncStatusInfo(status: TaskStatus.RanToCompletion, percentage: (int)100);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        data.SetSyncStatusInfo(status: TaskStatus.Faulted);
                    }

                }
            }, cancellationToken)).ToList();
            Task t = Task.WhenAll(tasks);
            try
            {
                await t;
            }
            catch
            {
                // ignored
            }
            return true;
        }
        public async Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
        {
            var actions = StartSyncActions.Select(a => Task.Run(() =>
            {
                a.Invoke(cancellationToken);
            }));
            _ = Task.WhenAll(actions);
            int maxConcurrency = TaskScheduler.Current.MaximumConcurrencyLevel;
            int maxTasks = 100;
            await UpdateSyncStatus(cancellationToken);
            var tasks = DataConnectors.Values.Select(data => Task.Run(async () =>
            {
                try
                {
                    int workers = DataConnectors.Values.Count(d => d.SyncStatusInfo.TaskStatus == TaskStatus.Running);
                    int retrieve = maxTasks / workers;
                    if (Properties.TryGetValue("LastSync", out var maxTasksProperty))
                    {
                        DateTime.TryParse(maxTasksProperty.Value.ToString(), out DateTime lastSync);
                        DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
                        if (lastSync > threeMonthsAgo)
                        {
                            await UpdateDb(cancellationToken);
                            data.SetSyncStatusInfo(
                                status: TaskStatus.RanToCompletion,
                                serverItemCount: GetDb(data).Value.GetTotalCount(),
                                percentage: 100);
                            return;
                        }
                    }

                    data.SyncStatusInfo.TaskStatus = TaskStatus.Running;
                    
                    BaseData[] baseData;
                    int newTotal = 0;
                    double newPercent = 0;
                    while (data.SyncStatusInfo.TaskStatus is TaskStatus.Running)
                    {
                        try
                        {
                            baseData = await data.GetAllAsync(
                                limit: retrieve,
                                startIndex: data.SyncStatusInfo.ServerItemCount,
                                setSortOrder: SortOrder.Descending,
                                setSortTypes: ItemSortBy.DateCreated,
                                cancellationToken: cancellationToken
                            );

                            newTotal = data.SyncStatusInfo.ServerItemCount + baseData.Length;
                            newPercent = ((double)newTotal / data.SyncStatusInfo.ServerItemTotal) * 100;

                            data.SetSyncStatusInfo(serverItemCount: newTotal, percentage: (int)newPercent);

                            // Download and set image code
                            
                            // if (Properties.TryGetValue("AppDataPath", out var appDataPath))
                            // {
                            //     var path = Path.Combine(appDataPath.Value.ToString(), "placeholder");
                            //     Blurhasher.DownloadMusicItemBitmap(baseData, GetDb(data).Value, path, 12, 12);
                            // }

                            GetDb(data).Value.InsertRange(baseData, cancellationToken);
                            if (baseData.Length < retrieve)
                            {
                                data.SetSyncStatusInfo(status: TaskStatus.RanToCompletion);
                                break;
                            }

                            workers = DataConnectors.Values.Where(d => d.SyncStatusInfo.TaskStatus == TaskStatus.Running).Count();
                            retrieve = maxTasks / workers;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                            Trace.TraceError(ex.StackTrace);
                            data.SetSyncStatusInfo(status: TaskStatus.Faulted);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Sync Failed: {ex.Message}");
                    Trace.WriteLine($"Sync Failed: {ex.StackTrace}");
                    data.SetSyncStatusInfo(status: TaskStatus.Faulted);
                }                
            }, cancellationToken)).ToList();
            Task t = Task.WhenAll(tasks);
            try
            {
                await t;
                Properties["LastSync"].Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                // ignored
            }
            return true;
        }
        public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<BaseData>());
        }
        public string GetAddress()
        {
            return (string)Properties["URL"].Value;
        }
        public string GetProfileImageUrl()
        {
            return "";
        }
        private async Task<bool> UpdateSyncStatus(CancellationToken cancellationToken = default)
        {
            var tasks = DataConnectors.Values.Select(data => Task.Run(async () =>
            {
                data.SetSyncStatusInfo(
                    TaskStatus.Running,
                    0,
                    await data.GetTotalCountAsync(),
                    0,
                    0);
                data.GetTotalCountAsync(cancellationToken: cancellationToken).Wait(cancellationToken);
            }, cancellationToken))
            .ToList();
            Task t = Task.WhenAll(tasks);
            try
            {
                await t;
            }
            catch
            {
                // ignored
            }
            return true;
        }
        private async Task<string?> GetUserViewId(string authHeader, string serverId, JfAuthRootInfo authInfo)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{serverId}/Users/{authInfo.User.Id}/Views"),
                Headers = { 
                    { "Accept", "application/json" },
                    { "User-Agent", $"{_authInfo.SessionInfo.Client}/{_authInfo.SessionInfo.ApplicationVersion}" },
                    { "Authorization", $"MediaBrowser Token=\"{authInfo.AccessToken}\", {authHeader}" }
                }
            };
            var response = await _httpClient.SendAsync(httpRequestMessage);
            string jsonContent = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(jsonContent);
            return resultObject.Items.First(d => d.CollectionType == "music").Id.ToString();
        }
        private KeyValuePair<MediaCapabilities, IDbItemConnector> GetDb(IMediaDataConnector mediaDataConnector)
        {
            if(_database == null)
            {
                throw new NullReferenceException("Database cannot be null!");
            }
            try
            {
                var returnVal = _database.Connectors.GetDataConnectors().First(d => d.Value.MediaType == mediaDataConnector.MediaType);
                return returnVal;
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}