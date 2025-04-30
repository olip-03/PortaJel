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
using Portajel.Connections.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Portajel.Connections.Services.Database;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin
{
    public class JellyfinServerConnector : IMediaServerConnector
    {
        private IDbConnector _database;
        private UserDto? _userDto;
        private SessionInfoDto? _sessionInfo;
        private JellyfinSdkSettings? _sdkClientSettings;
        private JellyfinApiClient? _jellyfinApiClient;
        public IMediaDataConnector AlbumData { get; set; } = null!;
        public IMediaDataConnector ArtistData { get; set; } = null!;
        public IMediaDataConnector SongData { get; set; } = null!;
        public IMediaDataConnector PlaylistData { get; set; } = null!;
        public IMediaDataConnector Genre { get; set; } = null!;
        public Dictionary<string, IMediaDataConnector> GetDataConnectors() => new()
        {
            { "Album", AlbumData },
            { "Artist", ArtistData },
            { "Song", SongData },
            { "Playlist", PlaylistData },
            { "Genre", Genre }
        };
        public Dictionary<MediaTypes, bool> SupportedReturnTypes { get; set; } =
            new()
            {
                { MediaTypes.Album, true },
                { MediaTypes.Artist, true },
                { MediaTypes.Song, true },
                { MediaTypes.Playlist, true },
                { MediaTypes.Genre, true },
            };
        public string Name { get; } = "JellyFin";
        public string Description { get; } = "Enables connections to the Jellyfin Media Server.";
        public string Image { get; } = "icon_jellyfin.png";
        public Dictionary<string, ConnectorProperty> Properties { get; set; } = new();
        public SyncStatusInfo SyncStatus { get; set; } = new();
        public List<Action<CancellationToken>> AuthenticateActions { get; set; } = new();
        public List<Action<CancellationToken>> StartSyncActions { get; set; } = new();
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
                        "AppName", new ConnectorProperty(
                            label: "App Name",
                            description: "The name of the Jellyfin Client Application.",
                            value: appName,
                            protectValue: false,
                            userVisible: true
                            )
                    },
                    {
                        "URL", new ConnectorProperty(
                            label: "Url",
                            description: "The URL of the Jellyfin Server",
                            value: url,
                            protectValue: false,
                            userVisible: true)
                    },
                    {
                        "Username", new ConnectorProperty(
                            label: "Username",
                            description: "Username for data at Url.",
                            value: username,
                            protectValue: false,
                            userVisible: true)
                    },
                    {
                        "Password", new ConnectorProperty(
                            label: "Password",
                            description: "User password for data at Url.",
                            value: password,
                            protectValue: true,
                            userVisible: true)
                    },

                    {
                        "AppVersion", new ConnectorProperty(
                            label: "App Version",
                            description: "The version of the Jellyfin Client Application.",
                            value: appVerison,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "DeviceName", new ConnectorProperty(
                            label: "Device Name",
                            description: "The name of the device running this Jellyfin Client Application.",
                            value: deviceName,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "DeviceID", new ConnectorProperty(
                            label: "Device Name",
                            description: "The name of the device running this Jellyfin Client Application.",
                            value: deviceId,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "LastSync", new ConnectorProperty(
                            label: "Last Sync",
                            description: "The last time a full sync ran for this data.",
                            value: url,
                            protectValue: false,
                            userVisible: false)
                    },
                    {
                        "AppDataPath", new ConnectorProperty(
                            label: "App Data Path",
                            description: "Application Data Path for storing files.",
                            value: appDataPath,
                            protectValue: false,
                            userVisible: false)
                    },
                };
        }
        public async Task<AuthResponse> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            // At the beginning of AuthenticateAsync method, add validation
            if (Properties["AppName"].Value == null ||
                Properties["AppVersion"].Value == null ||
                Properties["DeviceName"].Value == null ||
                Properties["DeviceID"].Value == null ||
                Properties["URL"].Value == null ||
                Properties["Username"].Value == null ||
                Properties["Password"].Value == null)
            {
                return new AuthResponse()
                {
                    IsSuccess = false,
                    Message = "Missing required properties for authentication"
                };
            }
            try
            {
                ServiceCollection serviceCollection = new ServiceCollection();
                serviceCollection.AddHttpClient("Default", c =>
                {
                    c.DefaultRequestHeaders.UserAgent.Add(
                        new ProductInfoHeaderValue(
                            (string)Properties["AppName"].Value.ToString(),
                            (string)Properties["AppVersion"].Value.ToString()));
                    c.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json, 1.0));
                    c.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                })
                    .ConfigurePrimaryHttpMessageHandler(_ => new SocketsHttpHandler
                    {
                        AutomaticDecompression = DecompressionMethods.All,
                        RequestHeaderEncodingSelector = (_, _) => Encoding.UTF8
                    });

                // Add Jellyfin SDK services.
                // include support for session.SupportsRemoteControl
                // See lines 326 for what Jellyfin-Web wants from clients, for remote functionality https://github.com/jellyfin/jellyfin-web/blob/e5df4dd56bc180dfa24a52a99c718459a4074d56/src/controllers/dashboard/dashboard.js#L324 
                serviceCollection.AddSingleton<JellyfinSdkSettings>();
                serviceCollection.AddSingleton<IAuthenticationProvider, JellyfinAuthenticationProvider>();
                serviceCollection.AddScoped<IRequestAdapter, JellyfinRequestAdapter>(s => new JellyfinRequestAdapter(
                    s.GetRequiredService<IAuthenticationProvider>(),
                    s.GetRequiredService<JellyfinSdkSettings>(),
                    s.GetRequiredService<IHttpClientFactory>().CreateClient("Default")));
                serviceCollection.AddScoped<JellyfinApiClient>();

                ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                _jellyfinApiClient = serviceProvider.GetRequiredService<JellyfinApiClient>();
                _sdkClientSettings = serviceProvider.GetRequiredService<JellyfinSdkSettings>();
                _sdkClientSettings.SetServerUrl(Properties["URL"].Value.ToString());
                _sdkClientSettings.Initialize(
                    (string)Properties["AppName"].Value.ToString(),
                    (string)Properties["AppVersion"].Value.ToString(),
                    (string)Properties["DeviceName"].Value.ToString(),
                    (string)Properties["DeviceID"].Value.ToString());

                var authenticationResult = await _jellyfinApiClient.Users.AuthenticateByName.PostAsync(
                    new AuthenticateUserByName
                    {
                        Username = Properties["Username"].Value.ToString(),
                        Pw = Properties["Password"].Value.ToString()
                    }, cancellationToken: cancellationToken).ConfigureAwait(false);

                if (authenticationResult != null)
                {
                    _sdkClientSettings.SetAccessToken(authenticationResult.AccessToken);
                    _userDto = authenticationResult.User;
                    _sessionInfo = authenticationResult.SessionInfo;
                    if(authenticationResult.AccessToken == null)
                    {
                        return new AuthResponse()
                        {
                            IsSuccess = false,
                            Message = $"API Error: Login Failure! Could not return Access Token. (Status: Failed)"
                        };
                    }
                }

                AlbumData = new JellyfinServerAlbumConnector(_jellyfinApiClient, _sdkClientSettings, _userDto);
                ArtistData = new JellyfinServerArtistConnector(_jellyfinApiClient, _sdkClientSettings, _userDto);
                SongData = new JellyfinServerSongConnector(_jellyfinApiClient, _sdkClientSettings, _userDto);
                PlaylistData = new JellyfinServerPlaylistConnector(_jellyfinApiClient, _sdkClientSettings, _userDto);
                Genre = new JellyfinServerGenreConnector(_jellyfinApiClient, _sdkClientSettings, _userDto);
            }
            catch (ApiException apiEx)
            {
                // More detailed API exception handling
                Trace.WriteLine($"Error: {apiEx.Message}");
                Trace.WriteLine($"Status code: {apiEx.ResponseStatusCode}");
                Trace.WriteLine($"Source: {apiEx.Source}");

                return new AuthResponse()
                {
                    IsSuccess = false,
                    Message = $"API Error: {apiEx.Message} (Status: {apiEx.ResponseStatusCode})"
                };
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                return new AuthResponse()
                {
                    IsSuccess = false,
                    Message = $"{ex.Message}"
                };
            }

            var actions = AuthenticateActions.Select(a => Task.Run(() =>
            {
                a.Invoke(cancellationToken);
            }));
            _ = Task.WhenAll(actions);
            return AuthResponse.Ok();
        }
        public async Task<bool> UpdateDb(CancellationToken cancellationToken = default)
        {
            await UpdateSyncStatus(cancellationToken);
            var tasks = GetDataConnectors().Values.Select(data => Task.Run(async () =>
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
                            data.SetSyncStatusInfo(status: TaskStatus.Faulted, percentage: (int)100);
                            continue;
                        }
                        if (checkCount >= retrieve)
                        {
                            data.SetSyncStatusInfo(status: TaskStatus.RanToCompletion, percentage: (int)100);
                            continue;
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
            int maxTasks = 500;
            await UpdateSyncStatus(cancellationToken);
            var tasks = GetDataConnectors().Values.Select(data => Task.Run(async () =>
            {
                try
                {
                    int workers = GetDataConnectors().Values.Where(d => d.SyncStatusInfo.TaskStatus == TaskStatus.Running).Count();
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
                    while (data.SyncStatusInfo.TaskStatus is TaskStatus.Running)
                    {
                        try
                        {
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

                            if (Properties.TryGetValue("AppDataPath", out var appDataPath))
                            {
                                Blurhasher.DownloadMusicItemBitmap(items, GetDb(data).Value, appDataPath.Value.ToString(), 128, 128);
                            }

                            GetDb(data).Value.InsertRange(items, cancellationToken);
                            if (items.Length < retrieve)
                            {
                                data.SetSyncStatusInfo(status: TaskStatus.RanToCompletion);
                                continue;
                            }

                            workers = GetDataConnectors().Values.Where(d => d.SyncStatusInfo.TaskStatus == TaskStatus.Running).Count();
                            retrieve = maxTasks / workers;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                            data.SetSyncStatusInfo(status: TaskStatus.Faulted);
                            continue;
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
        public async Task<bool> SetIsFavourite(Guid id, bool isFavourite, string serverUrl)
        {
            await Task.Delay(10);
            return false;
        }
        public Task<BaseData[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<BaseData>());
        }
        public string GetUsername()
        {
            return (string)Properties["Username"].Value;
        }
        public string GetPassword()
        {
            return (string)Properties["Password"].Value;
        }
        public string GetAddress()
        {
            return (string)Properties["URL"].Value;
        }
        public string GetProfileImageUrl()
        {
            return "";
        }
        public UserCredentials GetUserCredentials()
        {
            return new UserCredentials(_sdkClientSettings.ServerUrl, (string)Properties["Username"].Value,
                _userDto.Id.ToString(), (string)Properties["Password"].Value, _sessionInfo.Id,
                _sdkClientSettings.AccessToken);
        }
        public MediaServerConnection GetConnectionType()
        {
            return MediaServerConnection.Jellyfin;
        }
        private async Task<bool> UpdateSyncStatus(CancellationToken cancellationToken = default)
        {
            var tasks = GetDataConnectors().Values.Select(data => Task.Run(async () =>
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
        private KeyValuePair<string, IDbItemConnector> GetDb(IMediaDataConnector mediaDataConnector)
        {
            if(_database == null)
            {
                throw new NullReferenceException("Database cannot be null!");
            }
            try
            {
                var returnVal = _database.GetDataConnectors().First(d => d.Value.MediaType == mediaDataConnector.MediaType);
                return returnVal;
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}