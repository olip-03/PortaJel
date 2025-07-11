using System.Net.Http;
using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Newtonsoft.Json;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Jellyfin.Dto;
using Portajel.Connections.Structs;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Connections.Services.Jellyfin;

public class JellyfinConnectorTemplate : IMediaDataConnector
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _defaultHeaders;
    private readonly string _serverUrl;
    private readonly Guid _userId;

    public JellyfinConnectorTemplate(
        MediaType mediaType, 
        HttpClient httpClient,
        Dictionary<string, string> defaultHeaders,
        string serverUrl,
        Guid userId)
    {
        MediaType = mediaType;
        _httpClient = httpClient;
        _defaultHeaders = defaultHeaders;
        _serverUrl = serverUrl;
        _userId = userId;

        foreach (var header in defaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public MediaType MediaType { get; }
    public SyncStatusInfo SyncStatusInfo { get; set; } = new();

    public async Task<BaseData[]> GetAllAsync(
        int? limit = null,
        int startIndex = 0,
        bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album,
        SortOrder setSortOrder = SortOrder.Ascending,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null,
        string serverUrl = "",
        CancellationToken cancellationToken = default)
    {
        var apiUrl = BuildApiString(
            _serverUrl, 
            GetIncludeItemType(), 
            _userId.ToString(), 
            startIndex, 
            limit ?? 50,
            getFavourite: getFavourite,
            sortBy: setSortTypes,
            sortOrder: setSortOrder,
            includeIds: includeIds,
            excludeIds: excludeIds);

        using var request = CreateRequest(HttpMethod.Get, apiUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(content);
        
        return ReturnBuilder(resultObject, _serverUrl);
    }

    public async Task<BaseData> GetAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        var apiUrl = BuildApiString(
            _serverUrl,
            GetIncludeItemType(),
            _userId.ToString(),
            0,
            1,
            includeIds: [id]);

        using var request = CreateRequest(HttpMethod.Get, apiUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(content);
        
        return ReturnBuilder(resultObject, _serverUrl).First();
    }

    public async Task<BaseData[]> GetSimilarAsync(Guid id, int setLimit, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        var apiUrl = BuildSimilarApiString(_serverUrl, id, _userId, setLimit);
        
        using var request = CreateRequest(HttpMethod.Get, apiUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(content);
        
        return ReturnBuilder(resultObject, _serverUrl);
    }

    public async Task<int> GetTotalCountAsync(bool? getFavourite = null, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        var apiUrl = BuildApiString(
            _serverUrl,
            GetIncludeItemType(),
            _userId.ToString(),
            0,
            1,
            getFavourite: getFavourite,
            enableTotalRecordCount: true);

        using var request = CreateRequest(HttpMethod.Get, apiUrl);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(content);
        
        return resultObject?.TotalRecordCount ?? 0;
    }

    public Task<bool> DeleteAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid[] id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddRange(BaseData[] musicItems, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        foreach (var header in _defaultHeaders)
        {
            request.Headers.Add(header.Key, header.Value);
        }
        return request;
    }

    private string BuildApiString(
        string url, 
        string type, 
        string userId, 
        int startIndex, 
        int limit,
        Guid? parentId = null,
        bool? getFavourite = null,
        ItemSortBy? sortBy = null,
        SortOrder? sortOrder = null,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null,
        bool enableTotalRecordCount = false)
    {
        var apiUrl = $"{url}/Items?" +
                    $"userId={userId}&" +
                    $"startIndex={startIndex}&" +
                    $"limit={limit}&" +
                    $"recursive=true&" +
                    $"enableUserData=true&" +
                    $"includeItemTypes={type}&" +
                    $"enableTotalRecordCount={enableTotalRecordCount.ToString().ToLower()}&";

        if (parentId.HasValue)
            apiUrl += $"parentId={parentId}&";

        if (getFavourite.HasValue)
            apiUrl += $"isFavorite={getFavourite.Value.ToString().ToLower()}&";

        if (sortBy.HasValue)
            apiUrl += $"sortBy={sortBy}&";

        if (sortOrder.HasValue)
            apiUrl += $"sortOrder={sortOrder}&";

        if (includeIds?.Length > 0)
            apiUrl += $"ids={string.Join(",", includeIds)}&";

        if (excludeIds?.Length > 0)
            apiUrl += $"excludeItemIds={string.Join(",", excludeIds)}&";

        var fields = GetIncludeFields();
        if (fields.Length > 0)
            apiUrl += $"fields={string.Join(",", fields)}&";

        return apiUrl.TrimEnd('&');
    }

    private string BuildSimilarApiString(string url, Guid id, Guid userId, int limit)
    {
        string endpoint = MediaType switch
        {
            MediaType.Album => $"{url}/Albums/{id}/Similar",
            MediaType.Artist => $"{url}/Artists/{id}/Similar",
            _ => throw new NotImplementedException($"No Similar {MediaType} API is provided by the Jellyfin API")
        };

        return $"{endpoint}?userId={userId}&limit={limit}";
    }

    private string GetIncludeItemType()
    {
        return MediaType switch
        {
            MediaType.Album => "MusicAlbum",
            MediaType.Artist => "MusicArtist",
            MediaType.Song => "Audio",
            MediaType.AudioBook => "AudioBook",
            MediaType.Playlist => "Playlist",
            MediaType.Genre => "Genre",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string[] GetIncludeFields()
    {
        return MediaType switch
        {
            MediaType.Album => [
                "DateCreated",
                "DateLastSaved",
                "DateLastMediaAdded",
                "ProviderIds",
                "ExternalUrls"
            ],
            MediaType.Artist => ["DateCreated"],
            MediaType.Song => [],
            MediaType.Playlist => [],
            MediaType.Genre => [],
            MediaType.AudioBook => [],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private BaseData[] ReturnBuilder(JfItemsDto result, string serverUrl)
    {
        return MediaType switch
        {
            MediaType.Album => result.Items?.Select(dto => AlbumData.Builder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            // MediaType.Artist => result.Items?.Select(dto => ArtistData.Builder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            // MediaType.Song => result.Items?.Select(dto => SongData.Builder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            // MediaType.Playlist => result.Items?.Select(dto => PlaylistData.Builder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            // MediaType.Genre => result.Items?.Select(dto => GenreData.Builder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.AudioBook => [], // TODO: Implement AudioBooks :)
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}