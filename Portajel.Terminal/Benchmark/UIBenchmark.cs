using BenchmarkDotNet.Attributes;
using CommandLine;
using Microsoft.Kiota.Abstractions.Extensions;
using Portajel.Connections.Database;
using Portajel.Connections.Services.Database;
using ZLinq;

namespace Portajel.Terminal.Benchmark;

[MemoryDiagnoser(false)]
public class UIBenchmark
{
    [GlobalSetup]
    public void Setup()
    {
        // Option A: if you already have a Program.Initialize or similar:
        Program.InitializeDatabase();  
        
        // Option B: or create/configure your connector here directly:
        // _database = new DatabaseConnector("your‐connection‐string");
        // _database.Connectors = new ConnectorFactory(_database);
    }
}