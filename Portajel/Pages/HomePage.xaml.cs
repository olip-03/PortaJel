using CommunityToolkit.Maui.Core.Extensions;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
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
        IDbItemConnector? albumConnector = await AwaitInit();
        if (albumConnector != null)
        {
            var data = albumConnector.GetAll(
                limit: 50,
                setSortTypes: Jellyfin.Sdk.Generated.Models.ItemSortBy.DateCreated,
                setSortOrder: Jellyfin.Sdk.Generated.Models.SortOrder.Descending);
            string cacheDir = Path.Combine(FileSystem.Current.CacheDirectory, "Blurhash");
            var itemsToAdd = Blurhasher.DownloadMusicItemBitmap(data, albumConnector, cacheDir, 50, 50);
            AlbumData[] albums = itemsToAdd.Select(s => (AlbumData)s).ToArray();
            _viewModel.Sample.Clear();
            foreach (var album in albums)
            {
                _viewModel.Sample.Add(album);
            }
            PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }
    }

    private void ScrollViewMain_Scrolled(object? sender, ScrolledEventArgs e)
    { 
        Trace.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
    }
      
    private async void NavigateSettings(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }

    private async Task<IDbItemConnector?> AwaitInit()
    {
        IDbItemConnector toReturn = null;
        await Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    try
                    {
                        toReturn = _database.Connectors.Album;
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
        return toReturn;
    }
}

// Models/ImageItem.cs
public class ImageItem
{
    public string Source_ { get; set; }
}

public class HomePageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<BaseData> RecentlyPlayed { get; set; } = [];
    public ObservableCollection<AlbumData> RecentlyAdded { get; set; } = [];
    public ObservableCollection<AlbumData> FavouriteItems { get; set; } = [];

    public int PageMargin { get; set; } = 10;
    public ObservableCollection<AlbumData> Sample
    {
        get => RecentlyAdded;
        set
        {
            if (RecentlyAdded != value)
            {
                RecentlyAdded = value;
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