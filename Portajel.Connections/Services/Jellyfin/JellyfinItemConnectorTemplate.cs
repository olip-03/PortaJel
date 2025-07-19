using System.Buffers;
using System.Net.Http;
using System.Text;
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

public class JellyfinItemConnectorTemplate : IMediaDataConnector
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl;
    private readonly Guid _userId;
    private readonly string _serverParentId;
    private ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;
    public JellyfinItemConnectorTemplate(
        MediaType mediaType, 
        HttpClient httpClient,
        string serverUrl,
        string serverParentId,
        Guid userId)
    {
        MediaType = mediaType;
        _httpClient = httpClient;
        _serverUrl = serverUrl;
        _serverParentId = serverParentId;
        _userId = userId;
    }

    public MediaType MediaType { get; }
    public SyncStatusInfo SyncStatusInfo { get; set; } = new();

    public async Task<BaseData[]> GetAllAsync(
        int? limit = null, 
        int startIndex = 0, 
        bool? getFavourite = null,
        ItemSortBy setSortTypes = ItemSortBy.Album, 
        SortOrder setSortOrder = SortOrder.Ascending,
        Guid? parentId = null,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null, 
        string serverUrl = "", 
        CancellationToken cancellationToken = default
    )
    {
        var apiUrl = BuildApiString(
            _serverUrl, 
            GetIncludeItemType(), 
            _userId.ToString(), 
            startIndex, 
            limit: limit,
            parentId: parentId,
            getFavourite: getFavourite,
            sortBy: setSortTypes,
            sortOrder: setSortOrder,
            includeIds: includeIds,
            excludeIds: excludeIds);
        
        ReadOnlySpan<byte> resultByes = await _httpClient.GetByteArrayAsync(apiUrl, cancellationToken);
        string jsonStr = Encoding.UTF8.GetString(resultByes);
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(jsonStr);
        
        return ReturnBuilder(resultObject, _serverUrl);
    }

    public async Task<BaseData> GetAsync(Guid id, string serverUrl = "", CancellationToken cancellationToken = default)
    {
        // unavoidable. Maybe consider this on the stack, it's going to be called a fucking lot 
        var apiUrl = BuildApiString(
            _serverUrl,
            GetIncludeItemType(),
            _userId.ToString(),
            0,
            1,
            includeIds: [id]);

        ReadOnlySpan<byte> resultByes = await _httpClient.GetByteArrayAsync(apiUrl, cancellationToken);
        string jsonStr = Encoding.UTF8.GetString(resultByes);
        var resultObject = JsonConvert.DeserializeObject<JfItemsDto>(jsonStr);
        
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
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
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
        return request;
    }

    private string BuildApiString(
        string url = "",
        string type = "",
        string userId = "",
        int? startIndex = 0,
        int? limit = 0,
        Guid? parentId = null,
        bool? getFavourite = null,
        ItemSortBy? sortBy = null,
        SortOrder? sortOrder = null,
        Guid?[]? includeIds = null,
        Guid?[]? excludeIds = null,
        bool enableTotalRecordCount = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        }
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            queryParams.Add($"userId={userId}");
        }

        if (startIndex.HasValue)
        {
            queryParams.Add($"startIndex={startIndex.Value}");
        }

        if (limit.HasValue)
        {
            queryParams.Add($"limit={limit}");
        }
        queryParams.Add("recursive=true");
        queryParams.Add("enableUserData=true");
        if (!string.IsNullOrWhiteSpace(type))
        {
            queryParams.Add($"includeItemTypes={type}");
        }
        queryParams.Add($"enableTotalRecordCount={enableTotalRecordCount.ToString().ToLower()}");
        
        if (parentId.HasValue && parentId.Value != Guid.Empty)
        {
            queryParams.Add($"parentId={parentId}");
        }
        else
        {
            queryParams.Add($"parentId={_serverParentId}");
        }
        
        if (getFavourite.HasValue)
        {
            queryParams.Add($"isFavorite={getFavourite.Value.ToString().ToLower()}");
        }
        if (sortBy.HasValue)
        {
            queryParams.Add($"sortBy={sortBy.Value}");
        }
        if (sortOrder.HasValue)
        {
            queryParams.Add($"sortOrder={sortOrder.Value}");
        }

        if (includeIds != null && includeIds.Length > 0)
        {
            var validIncludeIds = includeIds
                .Where(g => g.HasValue && g.Value != Guid.Empty)
                .Select(g => g.Value.ToString("N"));

            var joinedIncludeIds = string.Join(",", validIncludeIds);
            if (!string.IsNullOrEmpty(joinedIncludeIds))
            {
                queryParams.Add($"ids={joinedIncludeIds}");
            }
        }

        if (excludeIds != null && excludeIds.Length > 0)
        {
            var validExcludeIds = excludeIds
                .Where(g => g.HasValue && g.Value != Guid.Empty)
                .Select(g => g.Value.ToString("N"));

            var joinedExcludeIds = string.Join(",", validExcludeIds);
            if (!string.IsNullOrEmpty(joinedExcludeIds))
            {
                queryParams.Add($"excludeItemIds={joinedExcludeIds}");
            }
        }
        
        var fields = GetIncludeFields();
        if (fields != null && fields.Length > 0)
        {
            queryParams.Add($"fields={string.Join(",", fields)}");
        }
        
        var apiUrl = $"{url.TrimEnd('/')}/Items?{string.Join("&", queryParams)}";
        if (MediaType == MediaType.Genre)
        {
            apiUrl = $"{url.TrimEnd('/')}/Genres?{string.Join("&", queryParams)}"; 
        }
        return apiUrl;
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
            MediaType.Genre => "MusicAlbum, MusicArtist, MusicGenre, Audio",
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
                "ExternalUrls",
                "Genres",
                "ParentId"
            ],
            MediaType.Artist => [
                "DateCreated",
                "ParentId",
                "ProviderIds", 
                "Overview"
            ],
            MediaType.Song => [
                "DateCreated",
                "DateLastMediaAdded",
                "ParentId",
                "ProviderIds",
                "ExternalUrls",
                "MediaStreams"
            ],
            MediaType.Playlist => [
                "DateCreated",
                "DateLastMediaAdded",
                "ParentId",
                "Path"
            ],
            MediaType.Genre => [],
            MediaType.AudioBook => [],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private BaseData[] ReturnBuilder(JfItemsDto result, string serverUrl)
    {
        return MediaType switch
        {
            MediaType.Album => result.Items?.Select(dto => JfBaseItemDto.AlbumBuilder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.Artist => result.Items?.Select(dto => JfBaseItemDto.ArtistBuilder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.Song => result.Items?.Select(dto => JfBaseItemDto.SongBuilder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.Playlist => result.Items?.Select(dto => JfBaseItemDto.PlaylistBuilder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.Genre => result.Items?.Select(dto => JfBaseItemDto.GenreBuilder(dto, serverUrl)).AsArray<BaseData>() ?? [],
            MediaType.AudioBook => [], // TODO: Implement AudioBooks :)
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}