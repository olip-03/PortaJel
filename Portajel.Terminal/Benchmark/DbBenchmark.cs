using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using BenchmarkDotNet.Attributes;
using CommandLine;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Kiota.Abstractions.Extensions;
using Newtonsoft.Json;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Services.Jellyfin.Dto;
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
    private static readonly ArrayPool<string> StringPool = ArrayPool<String>.Shared;
    private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;   
    private static readonly ArrayPool<BaseData> BaseDataPool = ArrayPool<BaseData>.Shared;
    public MediaType MediaType { get; } = MediaType.Album;

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
    
    [Benchmark]
    [Arguments(1)]
    [Arguments(10)]
    [Arguments(50)]
    public async Task ByteAllocationTest(int limit)
    {
        const int itemCount = 2;
        // pick an initial buffer size you know will handle most payloads
        int initialSize = 16 * 1024; 
        byte[] buffer = _pool.Rent(initialSize);

        try
        {
            for (int i = 0; i < limit; i++)
            {
                string url = BuildApiString(
                    "https://media.oli.fm",
                    "MusicAlbum",
                    "920896d5-d21b-4488-8f47-292603d7ecd3",
                    i,
                    itemCount
                );

                // stream the content so we never allocate a new array
                using var resp = await httpClient
                    .GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();

                await using var stream = await resp.Content
                    .ReadAsStreamAsync()
                    .ConfigureAwait(false);

                // read fully into 'buffer', growing only as needed
                int bytesRead = ReadToBufferAsync(stream, ref buffer);
                
                // now decode only the bytes we actually read
                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }
        finally
        {
            // return the last buffer we were using
            _pool.Return(buffer, clearArray: false);
        }
    }
    
    private static int ReadToBufferAsync(Stream stream, ref byte[] buffer)
    {
        const int chunk = 8 * 1024;
        int pos = 0;

        while (true)
        {
            // if we've filled the buffer, grow by a fixed chunk
            if (pos == buffer.Length)
            {
                int newSize = buffer.Length + chunk;
                byte[] bigger = _pool.Rent(newSize);
                Buffer.BlockCopy(buffer, 0, bigger, 0, pos);
                _pool.Return(buffer, clearArray: false);
                buffer = bigger;
            }

            int read = stream
                .Read(buffer, pos, buffer.Length - pos);
            if (read == 0) break;
            pos += read;
        }

        return pos;
    }

    [Benchmark]
    public async Task PoolAllocationTest()
    {
        var stringData = StringPool.Rent(1);
        for (int i = 0; i < 50; i++)
        {
            try
            {
                stringData[0] = BuildApiString(
                    "https://media.oli.fm",
                    "MusicAlbum",
                    "920896d5-d21b-4488-8f47-292603d7ecd3",
                    i,
                    1
                );
            }
            finally
            {
                StringPool.Return(stringData);
            }
        }
    }
    
    private async Task<string> GetUserView()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://media.oli.fm/Users/920896d5-d21b-4488-8f47-292603d7ecd3/Views");
        using var response = await httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        
        return "https://media.oli.fm/Users/920896d5-d21b-4488-8f47-292603d7ecd3/Views";
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
}