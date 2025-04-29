using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Portajel.Pages;

public partial class SearchPage : ContentPage
{
    private SearchPageViewModel _viewModel = new();
    private List<CancellationTokenSource> _cancellationTokens = new();
    private IDbConnector _database;
    public SearchPage(IDbConnector db)
	{
		_database = db;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Cancel any pending searches
        foreach (var token in _cancellationTokens)
        {
            if (!token.Token.IsCancellationRequested)
            {
                token.Cancel();
            }
        }
        
        var cToken = new CancellationTokenSource();
        _cancellationTokens.Add(cToken);
        _viewModel.IsLoading = true;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, cancellationToken: cToken.Token);
                BaseData[] result = await _database.SearchAsync(
                    searchTerm: e.NewTextValue,
                    limit: 50,
                    cancellationToken: cToken.Token);
                string cacheDir = Path.Combine(FileSystem.Current.CacheDirectory, "Blurhash");
                var itemsToAdd = Blurhasher.DownloadMusicItemBitmap(result.OfType<AlbumData>(), _database, cacheDir, 50, 50); 
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _viewModel.Albums.Clear();
                    foreach (var item in itemsToAdd)
                    {
                        if (item is not AlbumData album) continue;
                        _viewModel.Albums.Add(album);
                    }
                    _viewModel.IsLoading = false;
                });
            }
            catch (Exception ex)
            {
                Trace.Write($"Search Cancelled: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => _viewModel.IsLoading = false);
            }
        }, cToken.Token);
    }

}

public class SearchPageViewModel : INotifyPropertyChanged
{
    public bool IsLoading { get; set; } = false;

    private ObservableCollection<AlbumData> _albums = [];
    public int PageMargin = 10;
    public ObservableCollection<AlbumData> Albums
    {
        get => _albums;
        set
        {
            if (_albums != value)
            {
                _albums = value;
                OnAlbumsChange();
            }
        }
    }

    public SearchPageViewModel()
    {

    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnAlbumsChange([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}