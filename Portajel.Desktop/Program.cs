﻿using Avalonia;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Structs;
using Portajel.Desktop.Components.SettingsPanelViews;
using Portajel.Desktop.Pages;
using Portajel.Desktop.Structures.Services;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;
using Splat;

namespace Portajel.Desktop;

class Program
{
    public static CancellationTokenSource ClosingToken = new();
    
    public static RoutingState Router { get; } = new RoutingState();
    
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
        
        Trace.WriteLine($"AppDataPath: {AppDataPath}");
        
        var services = new ServiceCollection();
        var provider = services
                .AddSingleton<IDbConnector, DatabaseConnector>(serviceProvider => {
                    DatabaseConnector db = new DatabaseConnector(DbDataPath);
                    return db;
                })
                .AddSingleton<IServerConnector>(serviceProvider =>
                {
                    try
                    {
                        var jsonFilePath = Path.Combine(AppDataPath, "ServerConnector.json");
                        var json = "";
    
                        if (File.Exists(jsonFilePath))
                        {
                            json = File.ReadAllText(jsonFilePath);
                        }
    
                        var db = serviceProvider.GetRequiredService<IDbConnector>();
                        ServerConnectorSettings settings = new(json: json, db, AppDataPath);
                        return settings.ServerConnector;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                    }
                    return new ServerConnector();
                })
                .AddSingleton<LibraryViewCache>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<AddConnection>()
            .AddSingleton<MainWindow>()
            .BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);
        Locator.CurrentMutable.Register(() => new LibraryView(), typeof(IViewFor<LibraryViewModel>));
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
