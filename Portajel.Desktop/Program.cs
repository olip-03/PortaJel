using Avalonia;
using System;
using System.IO;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Desktop.Components.SettingsPanelViews;
using Portajel.Desktop.Structures.ViewModel;

namespace Portajel.Desktop;

class Program
{
    public static string AppDataPath = null!;
    public static string DbDataPath = null!;
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
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
        
        var services = new ServiceCollection();
        var provider = services
                .AddSingleton<IDbConnector, DatabaseConnector>(serviceProvider => {
                    DatabaseConnector db = new DatabaseConnector(DbDataPath);
                    return db;
                })
                .AddSingleton<ServerConnector>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<AddConnection>()
            .AddSingleton<MainWindow>()
            .BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseReactiveUI() 
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
