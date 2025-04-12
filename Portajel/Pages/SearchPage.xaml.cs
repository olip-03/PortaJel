using Portajel.Connections.Data;
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
                await Task.Delay(500, cancellationToken: cToken.Token);
                BaseMusicItem[] result = await _database.SearchAsync(
                    searchTerm: e.NewTextValue, 
                    limit: 50, 
                    cancellationToken: cToken.Token);
                _viewModel.Albums.Clear();
                _viewModel.IsLoading = false;
                foreach (var item in result)
                {
                    if (item is Album album)
                    {
                        _viewModel.Albums.Add(album);
                    }
                };
            }
            catch (Exception ex)
            {
                Trace.Write($"Search Cancelled: {ex.Message}");
            }
        }, cToken.Token);
    }
}

public class SearchPageViewModel : INotifyPropertyChanged
{
    public bool IsLoading { get; set; } = false;

    private ObservableCollection<Album> _albums = [];
    public int PageMargin = 10;
    public ObservableCollection<Album> Albums
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