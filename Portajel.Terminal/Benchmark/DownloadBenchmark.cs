using System.Buffers;
using BenchmarkDotNet.Attributes;
using CommandLine;
using Microsoft.Kiota.Abstractions.Extensions;
using Newtonsoft.Json;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Services.Jellyfin;
using Portajel.Connections.Services.Jellyfin.Dto;
using Portajel.Connections.Structs;

namespace Portajel.Terminal.Benchmark;

[MemoryDiagnoser(false)]
public class DownloadBenchmark
{
    private DatabaseConnector _database = Program.Database;
    private ServerConnector _server = Program.Server;

    private IMediaServerConnector _jfServer;
    
    private ArrayPool<BaseData> dataPool = ArrayPool<BaseData>.Shared; 
    
    [GlobalSetup]
    public void Setup()
    {
        Program.InitializeDatabase();  
        _database = Program.Database!;

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
        _jfServer = _server.Servers.First();
    }
    
    [Benchmark]
    public async Task<BaseData[]> GetAlbumData()
    {
        int total = await _jfServer.DataConnectors["Playlist"].GetTotalCountAsync();
        BaseData[] data = new BaseData[total];
        for (int i = 0; i < total; i++)
        {
            await _jfServer.DataConnectors["Playlist"].GetAllAsync(1, i).ContinueWith(d =>
            {
                data[i] = d.Result.First();
            });
        }
        return data;
    }

    [Benchmark]
    public async Task<BaseData[]> GetAlbumDataPooled()
    {
        int total = await _jfServer.DataConnectors["Playlist"].GetTotalCountAsync();
        // rent at least 'total' slots
        BaseData[] buffer = dataPool.Rent(total);
        try
        {
            for (int i = 0; i < total; i++)
            {
                // API Code 
                
                
                // await the single‐item page
                var page = await _jfServer
                    .DataConnectors["Playlist"]
                    .GetAllAsync(1, i);

                // stash the first (and only) item
                buffer[i] = page.First();
            }

            // copy into a perfectly sized array for return
            var result = new BaseData[total];
            Array.Copy(buffer, 0, result, 0, total);
            return result;
        }
        finally
        {
            // hand the oversized buffer back to the pool
            dataPool.Return(buffer, clearArray: false);
        }
    }
}