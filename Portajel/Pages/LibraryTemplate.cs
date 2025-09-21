using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;
using Portajel.Structures.Converters;
using Portajel.Structures.ViewModels.Pages.Library;
using SelectionMode = Microsoft.Maui.SelectionMode;

namespace Portajel.Pages.Library;

public class LibraryTemplate : ContentPage
{
    private readonly ToolbarItem _toolbarFav  = new()
    {
        Text = "Favourites"
    };
    private readonly VirtualListView _vlv = new()
    {
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill,
        IsRefreshEnabled = true,
        SelectionMode = SelectionMode.None
    };
    
    private DatabaseBindViewModel? _vm;
    private readonly MediaType _type;
    private readonly IDbConnector _db;

    public LibraryTemplate(IDbConnector table, MediaType type)
    {
        _type = type;
        _db = table;
        BindingContext = null;
        Render();
    }

    protected override void OnAppearing()
    {
        UpdateColor();
        if (BindingContext == null)
        {
            switch (_type)
            {
                case MediaType.Album:
                    _vm = new(_db.Connectors.Album);
                    break;
                case MediaType.Artist:
                    _vm = new(_db.Connectors.Artist);
                    break;
                case MediaType.Playlist:
                    _vm = new(_db.Connectors.Playlist);
                    break;
                case MediaType.Song:
                    _vm = new(_db.Connectors.Song);
                    break;
                case MediaType.Genre:
                    _vm = new(_db.Connectors.Genre);
                    break;
                default:
                    throw new ArgumentException($"Unsupported media type: {_type}");
            }
            _vm.OnDataRefresh += OnDataRefresh;
            BindingContext = _vm;
        }
        base.OnAppearing();
    }

    private async void OnDataRefresh(object? sender, EventArgs e)
    {
        _vlv.Adapter = new MemoryItemAdaptor();
        _vlv.Opacity = 0;
        _vlv.Adapter = _vm?.Adapter;
        await _vlv.FadeTo(1, 333, Easing.CubicOut);
    }

    private void Render()
    {
        Shell.SetNavBarIsVisible(this, true);
        UpdateColor();
        
        switch (_type)
        {
            case MediaType.Album:
                Title = "Album";
                break;
            case MediaType.Artist:
                Title = "Album";
                break;
            case MediaType.Playlist:
                Title = "Album";
                break;
            case MediaType.Song:
                Title = "Album";
                break;
            case MediaType.Genre:
                Title = "Album";
                break;
            default:
                Title = $"{_type}";
                break;
        }

        var resourceDictionary = new ResourceDictionary();
        resourceDictionary.Add("ItemTemplateSelector", new ListTemplateSelector());
        resourceDictionary.Add("ImageUrlConverter", new ImageUrlConverter());
        Resources = resourceDictionary;

        _toolbarFav.SetBinding(MenuItem.IconImageSourceProperty, "FavouriteIcon");
        _toolbarFav.Clicked += Favourite_OnClicked;

        var toolbarOrder = new ToolbarItem
        {
            Text = "Order",
            IconImageSource = "arrow_downward.png"
        };
        toolbarOrder.SetBinding(MenuItem.IconImageSourceProperty, "OrderIcon");
        toolbarOrder.Clicked += Order_OnClicked;

        var toolbarFilter = new ToolbarItem
        {
            Text = "Filter",
            IconImageSource = "filter_list.png"
        };
        toolbarFilter.Clicked += Filter_OnClicked;

        ToolbarItems.Add(_toolbarFav);
        ToolbarItems.Add(toolbarOrder);
        ToolbarItems.Add(toolbarFilter);

        _vlv.SetBinding(VirtualListView.RefreshCommandProperty, "RefreshCommand");
        _vlv.SetBinding(VirtualListView.AdapterProperty, "Adapter");
        _vlv.ItemTemplateSelector = (ListTemplateSelector)Resources["ItemTemplateSelector"];

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Add VirtualListView to Grid
        grid.Children.Add(_vlv);

        // Set Content
        Content = grid;
    }

    private void UpdateColor()
    {
        if (Application.Current != null)
        {
            var rd = Application.Current?.Resources;
            rd.TryGetValue("BackgroundColor", out var brushOrColor);
            Color color = (Color)brushOrColor;
            Shell.SetBackgroundColor(this, color);
        }
    }

    private void Favourite_OnClicked(object? sender, EventArgs e)
    {
        if (_vm != null) _vm.FilterFavourites = !_vm.FilterFavourites;
    }

    private void Order_OnClicked(object? sender, EventArgs e)
    {
        if (_vm != null) _vm.OrderDescending = !_vm.OrderDescending;
    }

    private void Filter_OnClicked(object? sender, EventArgs e)
    {
        // Throw modal to allow filter selection
    }
}