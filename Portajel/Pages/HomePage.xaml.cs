using CommunityToolkit.Maui.Core.Extensions;
using Portajel.Connections.Data;
using Portajel.Connections.Interfaces;
using Portajel.Pages.Settings;
using RTools_NTS.Util;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Portajel.Pages;

public partial class HomePage : ContentPage
{
    private HomePageViewModel _viewModel = new();
    private IDbConnector _database; 
	public HomePage(IDbConnector db)
	{
        _database = db;
        
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            _viewModel.PageMargin = 0;
        }
        BindingContext = _viewModel;
        Init();
    }
    private async void Init()
    {
        IDbItemConnector albumConnector = null!;
        await Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    try
                    {
                        albumConnector = _database.GetDataConnectors()["Album"];
                        break;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write($"Loading Home Data Failed: {ex.Message}");
            }
        });

        var data = await albumConnector.GetAllAsync(
                    limit: 50,
                    setSortTypes: Jellyfin.Sdk.Generated.Models.ItemSortBy.DateCreated,
                    setSortOrder: Jellyfin.Sdk.Generated.Models.SortOrder.Descending);

        // Convert all at once
        Album[] albums = data.Select(s => (Album)s).ToArray();
        _viewModel.Sample.Clear(); // Optional: clear existing items
        foreach (var album in albums)
        {
            _viewModel.Sample.Add(album);
        }
        PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
    }
    private void ScrollViewMain_Scrolled(object? sender, ScrolledEventArgs e)
    {
        
        Trace.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
    }
    private async void NavigateSettings(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }
}

// Models/ImageItem.cs
public class ImageItem
{
    public string Source_ { get; set; }
}

public class HomePageViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Album> _music = [];
    public int PageMargin = 10;
    public ObservableCollection<Album> Sample
    {
        get => _music;
        set
        {
            if (_music != value)
            {
                _music = value;
                OnPropertyChanged();
            }
        }
    }

    public HomePageViewModel()
    {
        
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}