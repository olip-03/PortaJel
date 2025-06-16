using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;

namespace Portajel.Desktop;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
        
        this.Activated += OnWindowActivated;
        this.Deactivated += OnWindowDeactivated;
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
}