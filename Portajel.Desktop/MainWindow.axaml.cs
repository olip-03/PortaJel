using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using Portajel.Connections;
using Portajel.Desktop.Components;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;
using SettingsIndex = Portajel.Desktop.Components.SettingsPanelViews;

namespace Portajel.Desktop;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private ServerConnector _server = Ioc.Default.GetService<ServerConnector>();
    private Components.SettingsPanel? _settingsPanel;
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
        
        _settingsPanel = this.FindControl<Components.SettingsPanel>("SettingsPanel");
        
        this.Activated += OnWindowActivated;
        this.Deactivated += OnWindowDeactivated;
        
        _ = Task.Run(async () =>
        {
            await _server.AuthenticateAsync(Program.ClosingToken.Token);
            await _server.StartSyncAsync(Program.ClosingToken.Token);
        });
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Program.ClosingToken.Cancel();
        base.OnClosing(e);
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        // Window gained focus
        string colour = "#31363B";
        Color color = Color.FromRgb(
            Convert.ToByte(colour.Substring(1, 2), 16),
            Convert.ToByte(colour.Substring(3, 2), 16),
            Convert.ToByte(colour.Substring(5, 2), 16));
        ViewModel.PanelColor = new SolidColorBrush()
        {
            Color = color,
            Opacity = 1
        };
        ViewModel.TestString = "Active";
    }
    
    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        // Window lost focus
        string colour = "#2A2E32";
        Color color = Color.FromRgb(
            Convert.ToByte(colour.Substring(1, 2), 16),
            Convert.ToByte(colour.Substring(3, 2), 16),
            Convert.ToByte(colour.Substring(5, 2), 16));
        ViewModel.PanelColor = new SolidColorBrush()
        {
            Color = color,
            Opacity = 1
        };
        ViewModel.TestString = "Unactive";
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_settingsPanel.Opacity == 0)
        {
            _settingsPanel.Opacity = 1;
            _settingsPanel.IsHitTestVisible = true;
            
        }
        else
        {
            _settingsPanel.Opacity = 0;
            _settingsPanel.IsHitTestVisible = false;
        }
    }
}