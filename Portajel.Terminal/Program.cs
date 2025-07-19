using System.Text.Json;
using BenchmarkDotNet.Running;
using CommandLine;
using Jellyfin.Sdk.Generated.LiveTv.Recordings.Folders;
using Newtonsoft.Json.Linq;
using Portajel.Terminal.View;
using Portajel.Terminal.Struct.Interface;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Services.Database;
using Portajel.Terminal.Benchmark;
using Portajel.Terminal.Struct;

namespace Portajel.Terminal
{
    class Program
    {
        public static string AppDataPath;
        public static string DbDataPath;

        public static DatabaseConnector Database;
        public static ServerConnector Server = new();

        private static CancellationTokenSource _refreshCancelToken = new();
        private static Stack<IView> _view = new();

        private static bool pauseRefresh = false;
        static async Task Main(string[] args)
        {
            DownloadBenchmark benchmark = new();
            benchmark.Setup();
            var result = await benchmark.GetAlbumData();
            // var test = benchmark.GetBlurhashBitmap();
            // await benchmark.DownloadAndSave();
            //
            // var baseData = await Server.Servers.First().DataConnectors["Genre"].GetAllAsync(limit: 10, startIndex: 200);
            // var media = baseData.Cast<GenreData>();
            // var json = JsonSerializer.Serialize(media);
            //
            // JToken jt = JToken.Parse(json);
            // string formatted = jt.ToString(Newtonsoft.Json.Formatting.Indented);
            //
            // Console.WriteLine(formatted);
            var summary = BenchmarkRunner.Run<DownloadBenchmark>();
        }

        public static void InitializeDatabase()
        {
            FolderCheck();
            Database = new(DbDataPath);
        }

        public static void LaunchInteractive()
        {
            FolderCheck();
            
            Database = new(DbDataPath);
            
            _view.Push(new MainView(Server));

            StartRefreshCoroutine(200);
            while (true)
            {

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Q)
                {
                    if (_view.Count == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("See you later! ");
                        Environment.Exit(0);
                        break;
                    }
                    _view.Pop();
                }

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (_view.First().Selected > 0)
                        {
                            _view.First().Selected -= 1;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (_view.First().Selected < _view.First().Selections.Count - 1)
                        {
                            _view.First().Selected += 1;
                        }
                        break;
                    case ConsoleKey.Enter:
                        _view.First().Selections.ElementAt(_view.First().Selected).Value.Invoke();
                        break;
                }
                
                if (!pauseRefresh)
                {
                    Refresh();
                }
                Task.Delay(100);
            }
            _refreshCancelToken.Cancel();
        }

        public static void SetView(IView view)
        {
            _view.Push(view);
        }

        public static void Back()
        {
            _view.Pop();
        }

        private static void FolderCheck()
        {
            string basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            );
            AppDataPath = Path.Combine(basePath, "portajel");
            DbDataPath = Path.Combine(AppDataPath, "portajel.db");
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
        }
        
        private static void Refresh()
        {
            Console.Clear();

            if (_view.First().ShowTitle)
            {
                PrintTitle();
            }

            Console.WriteLine();

            pauseRefresh = true;
            if (!_view.Any())
            {
                Console.WriteLine();
                Console.WriteLine("See you later! ");
                Environment.Exit(0);
            }
            if (!_view.First().FormSubmitted)
            {
                foreach (var formItem in _view.First().Form)
                {
                    formItem.Query();
                }

                _view.First().FormSubmitted = true;
            }

            pauseRefresh = false;

            PrintContents();
            Console.WriteLine();

            PrintSelections();
        }

        private static void PrintTitle()
        {
            foreach (var row in _view.First().Title)
            {
                Console.WriteLine(row);
            }
        }

        private static void PrintContents()
        {
            foreach (var row in _view.First().Contents)
            {
                Console.WriteLine(row);
            }
        }

        private static void PrintSelections()
        {
            int i = 0;
            int skip = 0;
            foreach (var row in _view.First().Selections)
            {
                if (string.IsNullOrWhiteSpace(row.Key))
                {
                    Console.WriteLine();
                    skip++;
                    i++;
                    continue;
                }
                if (i == _view.First().Selected)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" {i - skip} > {row.Key}");
                    Console.ResetColor(); // Reset to default color
                }
                else
                {
                    Console.WriteLine($" {i- skip} - {row.Key}");
                }
                i++;
            }
        }

        private static void StartRefreshCoroutine(int delay)
        {
            _ = Task.Run(async () =>
            {
                while (!_refreshCancelToken.IsCancellationRequested)
                {
                    if (!pauseRefresh)
                    {
                        Refresh();
                    }
                    await Task.Delay(delay);
                }
            });
        }
    }
}
