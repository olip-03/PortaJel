using BenchmarkDotNet.Attributes;
using CommandLine;
using Microsoft.Kiota.Abstractions.Extensions;
using Newtonsoft.Json;
using Portajel.Connections;
using Portajel.Connections.Database;
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
    }

    [Benchmark]
    public async Task<BaseData[]> GetAlbumData()
    {
        var result = await _server.Servers.First().DataConnectors["Album"].GetAllAsync(1, 250);
        return result;
    }
}