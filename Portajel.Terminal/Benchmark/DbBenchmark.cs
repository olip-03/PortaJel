using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using CommandLine;
using Jellyfin.Sdk.Generated.Models;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Kiota.Abstractions.Extensions;
using Newtonsoft.Json;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Structs;
using Portajel.Terminal.Struct.MessagePack;
using ZLinq;

namespace Portajel.Terminal.Benchmark;

[MemoryDiagnoser(false)]
public class DbBenchmark
{
    private DatabaseConnector _database = Program.Database;
    private ServerConnector _server = Program.Server;
    private HttpClient httpClient = new HttpClient();
    private readonly Dictionary<string, string> _defaultHeaders = new()
    {
        { "accept", "application/json" },
        { "Authorization", "Mediabrowser Token=\"d3aa895e30534bb88df149f762409f46\"" }
    };
    
    [GlobalSetup]
    public void Setup()
    {
        Program.InitializeDatabase();  
        _database = Program.Database!;

        httpClient.DefaultRequestHeaders.Add("accept", "application/json"); 
        httpClient.DefaultRequestHeaders.Add("Authorization", "Mediabrowser Token=\"d3aa895e30534bb88df149f762409f46\""); 
        
        JellyfinServerConnector jf = new(
            _database,
            "https://media.oli.fm",
            "local",
            "test1234",
            "PortaJel-Benchy",
            "0.0.1",
            "Benchy",
            "Benchy",
            Program.AppDataPath);
        _server.AddServer(jf);
        var authTask = _server.AuthenticateAsync();
        authTask.Wait();
    }

    // [Benchmark]
    // [Arguments(1)]
    // [Arguments(10)]
    // [Arguments(50)]
    // [Arguments(100)]
    // public async Task<BaseData[]> DbGetAlbumNew(int limit)
    // {
    //     string selector = "Album";
    //     BaseData[] result = await _server.Servers.First().DataConnectors[selector].GetAllAsync(limit: limit);
    //     return result;
    // }

    [Benchmark] 
    [Arguments(1)]
    [Arguments(10)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task<BaseData[]> GetArtistData(int limit)
    {
        await GetUserView();

        return await _server.Servers.First().DataConnectors["Artist"].GetAllAsync(limit: limit);
    }

    private async Task<string> GetUserView()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://media.oli.fm/Users/920896d5-d21b-4488-8f47-292603d7ecd3/Views");
        using var response = await httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        
        return "https://media.oli.fm/Users/920896d5-d21b-4488-8f47-292603d7ecd3/Views";
    }
    
    private async Task<byte[]> GetAlbumJsonUtf8Array(int startIndex, int limit)
    {
        var api = BuildApiString("https://media.oli.fm", "MusicAlbum","920896d5-d21b-4488-8f47-292603d7ecd3",startIndex,  limit);
        // using var albumReq = CreateRequest(HttpMethod.Get, api);
        var response = await httpClient.GetByteArrayAsync(api).ConfigureAwait(false);
        return response;
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
    
    private string BuildApiString(string url, string type, string userId, int startIndex, int limit, Guid? parentId = null)
    {
        return $"{url}/Items?" +
               $"userId={userId}&" +
               $"startIndex={startIndex}&" +
               $"limit={limit}&" +
               $"parentId={parentId}&" +
               $"recursive=true&" +
               $"enableUserData=true&" +
               $"includeItemTypes={type}&" +
               $"enableTotalRecordCount=true&";
    }
    
    private AlbumData[] BuildAlbumData(BaseItemDtoQueryResult data)
    {
        return null;
    }
}