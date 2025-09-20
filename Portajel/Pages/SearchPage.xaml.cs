using System.Collections.Concurrent;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Portajel.Connections.Enum;
using Portajel.Structures.Adaptor;
using RefreshEventArgs = Microsoft.Maui.RefreshEventArgs;

namespace Portajel.Pages;

public partial class SearchPage : ContentPage
{
    private readonly SearchPageViewModel _viewModel;
    private readonly List<CancellationTokenSource> _cancellationTokens = new();
    private readonly IDbConnector _database;
    
    private string _lastSearch = "";
    
    public SearchPage(IDbConnector db)
    {
        _viewModel = new();
		_database = db;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.Suggestions.Clear();
        _lastSearch = e.NewTextValue;
        
        CancelPending();
        
        if (String.IsNullOrWhiteSpace(e.NewTextValue))
        {
            _viewModel.Suggestions.Clear();
            // todo, base items are recently selected 
            Vlv.Adapter = new MemoryItemAdaptor();
            return; 
        }
        Vlv.Adapter = new MemoryItemAdaptor();
        _viewModel.IsLoading = true;
        Vlv.Opacity = 0;
        
        _ = Task.Run(async () =>
        {
            await DelayIfPending(300);
            var cToken = InitNewLoad();
            try
            {
                _viewModel.Suggestions.Clear();

                ConcurrentBag<BaseData> searchData = new();
                List<Task> queryTasks = new();
                foreach (var connector in _database.Connectors)
                {
                    queryTasks.Add(Task.Run(() =>
                    {
                        var items = connector.Value.Search(e.NewTextValue);
                        foreach (var item in items)
                        {
                            searchData.Add(item);
                            cToken.Token.ThrowIfCancellationRequested();
                        }
                    }, cToken.Token));
                }

                await Task.WhenAll(queryTasks);

                var ordered = searchData
                    .OrderByDescending(d => d.Name.ToLower().StartsWith(e.NewTextValue.ToLower()))
                    .ThenBy(d => d.MediaType switch
                    {
                        MediaType.Playlist => 1,
                        MediaType.Artist => 2,
                        MediaType.Album => 3,
                        MediaType.Genre => 4,
                        MediaType.Song => 5,
                        _ => 6
                    })
                    .ThenBy(d => d.Name);
                cToken.Token.ThrowIfCancellationRequested();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (e.NewTextValue == _lastSearch)
                    {
                        _viewModel.IsLoading = false;
                        Vlv.Adapter = new MemoryItemAdaptor()
                        {
                            Items = ordered.ToList()
                        };
                        Vlv.IsVisible = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Search Cancelled: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => _viewModel.IsLoading = false);
            }
            finally
            {
                _cancellationTokens.Remove(cToken);
                await Vlv.FadeTo(1, 250, Easing.CubicIn);
            }
        });
    }

    private void CancelPending()
    {
        foreach (var token in _cancellationTokens)
        {
            if (!token.Token.IsCancellationRequested)
            {
                token.Cancel();
            }
        }
    }

    private CancellationTokenSource InitNewLoad()
    {
        var cToken = new CancellationTokenSource();
        _cancellationTokens.Add(cToken);
        _viewModel.IsLoading = true;
        return cToken;
    }

    private async Task<bool> DelayIfPending(int by)
    {
        if (_cancellationTokens.Any())
        {
            await Task.Delay(by);
            return true;
        }

        return false;
    }

    private async void SearchBar_OnSearchButtonPressed(object? sender, EventArgs e)
    {
        // await Shell.Current.GoToAsync("album", new Dictionary<string, object>
        // {
        //     { "Properties", ItemData }
        // });
    }

    private async void Vlv_OnOnRefresh(object? sender, RefreshEventArgs e)
    {
        // refresh MusicBrainz and other online search 
        if (sender is VirtualListView vlv)
        {
            
        }
        
        e.Complete.Invoke();
    }
}

public class SearchPageViewModel : INotifyPropertyChanged
{
    public bool IsLoading { get; set; } = false;
    public MemoryItemAdaptor? Adapter { get; set; }

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

    public ObservableCollection<ListData> Suggestions { get; set; } = new();

    public SearchPageViewModel()
    {
        Adapter = new MemoryItemAdaptor();
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnAlbumsChange([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ListData(Guid key, string name, MediaType type)
{
    public Guid Key { get; set; } = key;
    public String Name { get; set; } = name;
    public MediaType Type { get; set; } = type;
}