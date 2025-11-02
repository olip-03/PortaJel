using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Jellyfin.Sdk.Generated.Models;
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
using SQLite;
using ZLinq;

namespace Portajel.Terminal.Benchmark;

[MemoryDiagnoser(false)]
public class DbSpeedBenchmark
{
    private DatabaseConnector _database = Program.Database;
    private ServerConnector _server = Program.Servers;
    private IMediaServerConnector _jfServer;

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
        _server.Add(jf);
        var authTask = _server.AuthenticateAsync();
        authTask.Wait();
        _jfServer = _server.First();
    }
    
    [Benchmark]
    public SongData SlowCall()
    {
        if (_database.Connectors.Playlist is TableQuery<SongData> songTable)
        {
            var sql = @"SELECT Id, Name, ArtistNames, ImgBlurhash, ImgSource FROM SongData 
                        WHERE (@favs IS NULL OR IsFavourite = @favs)
                        ORDER BY Name" + "DESC" + @"
                        LIMIT 1 OFFSET @itemIndex";
            return songTable.Connection.Query<SongData>(sql, false, 0).FirstOrDefault();
        }
        return null;
    }
    
    [Benchmark]
    public SongData FastCall()
    {
        if (_database.Connectors.Playlist is TableQuery<SongData> songTable)
        {
            var songSql = @"
                        SELECT t.* 
                        FROM (
                            SELECT Id
                            FROM SongData
                            WHERE (@favs IS NULL OR IsFavourite = @favs)
                            ORDER BY Name " + "DESC" + @"
                            LIMIT 1 OFFSET @itemIndex
                        ) q
                        JOIN SongData t ON t.Id = q.Id";
            return songTable.Connection.Query<SongData>(songSql, false, 0).FirstOrDefault();
        }

        return null;
    }
    
    // [Benchmark]
    // [Arguments(1)]
    // [Arguments(10)]
    // [Arguments(20)]
    // [Arguments(30)]
    // [Arguments(40)]
    // [Arguments(50)]
    // [Arguments(60)]
    // [Arguments(80)]
    // [Arguments(90)]
    // [Arguments(100)]
    // public void ArtistCalls(int limit)
    // {
    //     _database.Connectors.Artist.GetAll(limit, startIndex: 50);
    //     var total = _database.Connectors.Artist.GetTotalCount();
    //     
    //     Console.WriteLine($"collecting {limit}, returned {total} ArtistCalls!");
    // }
    //
    // [Benchmark]
    // [Arguments(1)]
    // [Arguments(10)]
    // [Arguments(20)]
    // [Arguments(30)]
    // [Arguments(40)]
    // [Arguments(50)]
    // [Arguments(60)]
    // [Arguments(80)]
    // [Arguments(90)]
    // [Arguments(100)]
    // public void GenreCalls(int limit)
    // {
    //     _database.Connectors.Genre.GetAll(limit, startIndex: 50);
    //     var total = _database.Connectors.Genre.GetTotalCount();
    //     
    //     Console.WriteLine($"collecting {limit}, returned {total} GenreCalls!");
    // }
    //
    // [Benchmark]
    // [Arguments(1)]
    // [Arguments(10)]
    // [Arguments(20)]
    // [Arguments(30)]
    // [Arguments(40)]
    // [Arguments(50)]
    // [Arguments(60)]
    // [Arguments(80)]
    // [Arguments(90)]
    // [Arguments(100)]
    // public void AlbumCalls(int limit)
    // {
    //     _database.Connectors.Album.GetAll(limit, startIndex: 50);
    //     var total = _database.Connectors.Album.GetTotalCount();
    //     
    //     Console.WriteLine($"collecting {limit}, returned {total} AlbumCalls!");
    // }
    //
    // [Benchmark]
    // [Arguments(1)]
    // [Arguments(10)]
    // [Arguments(20)]
    // [Arguments(30)]
    // [Arguments(40)]
    // [Arguments(50)]
    // [Arguments(60)]
    // [Arguments(80)]
    // [Arguments(90)]
    // [Arguments(100)]
    // public void SongCall(int limit)
    // {
    //     _database.Connectors.Song.GetAll(limit, startIndex: 50);
    //     var total = _database.Connectors.Song.GetTotalCount();
    //     
    //     Console.WriteLine($"collecting {limit}, returned {total} SongCall!");
    // }
}