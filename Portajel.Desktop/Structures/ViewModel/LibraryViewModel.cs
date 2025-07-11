using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DynamicData;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Structs;
using Portajel.Desktop.Components;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel
{
    private IDbItemConnector _dbItemConnection;
    public IDbItemConnector DbItemConnection { get => _dbItemConnection; }
    public RoutingState Router { get; } = new RoutingState();
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    
    public ReactiveCommand<Unit, IRoutableViewModel> GoToAlbum { get; }
    public ObservableCollection<BaseData> Items { get; } = new();
    public ObservableCollection<DataGridColumn> DataGridColumns { get; } = new();
    public int PageNumber
    {
        get => _pageNumber;
        set => this.RaiseAndSetIfChanged(ref _pageNumber, value);
    }
    private int _pageNumber = 1;
    public int MaxPageNumber
    {
        get => _maxPageNumber;
        set => this.RaiseAndSetIfChanged(ref _maxPageNumber, value);
    }
    private int _maxPageNumber = 1;
    public LibraryViewModel(IScreen screen) => HostScreen = screen;
    public LibraryViewModel(IScreen screen, IDbItemConnector dbItemConnection)
    {
        HostScreen = screen;
        _dbItemConnection = dbItemConnection ?? throw new ArgumentNullException(nameof(dbItemConnection));
        
        GenerateColumns(_dbItemConnection.MediaType);
    }
    private void GenerateColumns(MediaType mediaType)
    {
        DataGridColumns.Clear();
        switch (mediaType)
        {
            case MediaType.Album:
                DataGridColumns.Add(CreateImageColumn("Image", "ImgBlurhashSource", "ImgSource"));
                DataGridColumns.Add(CreateTextColumn("Name", "Name", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Artists", "ArtistNames", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaType.Artist:
                DataGridColumns.Add(CreateImageColumn("Image", "ImgBlurhashSource", "ImgSource"));
                DataGridColumns.Add(CreateTextColumn("Name", "Name", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaType.Song:
                DataGridColumns.Add(CreateImageColumn("Image", "ImgBlurhashSource", "ImgSource"));
                DataGridColumns.Add(CreateTextColumn("Name", "Name", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Artist", "ArtistNames", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaType.Genre:
                DataGridColumns.Add(CreateImageColumn("Image", "ImgBlurhashSource", "ImgSource"));
                DataGridColumns.Add(CreateTextColumn("Name", "Name", "selectable"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
        }
    }
    private DataGridTextColumn CreateTextColumn(string header, string bindingPath, string useClass = "")
    {
        var colu = new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(bindingPath),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            CellStyleClasses = { useClass },
            FontSize = 16,
        };
        return colu;
    }

    private void ColuOnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        
    }

    private DataGridTemplateColumn CreateImageColumn(string header, string placeholderBinding, string sourceBinding)
    {
        var imgTemplate = new FuncDataTemplate<object>((data, namescope) =>
        {
            var placeholder = new Image
            {
                Margin = new Thickness(4),
                ZIndex = 2,
                Height = 48,
                Width = 48,
            };
            placeholder.Bind(
                Image.SourceProperty,
                new Binding(placeholderBinding)
                {
                    Mode      = BindingMode.OneWay,
                    Converter = new FileUriToBitmapConverter(),
                });
            var image = new Image
            {
                Margin = new Thickness(4),
                ZIndex = 2,
                Height = 48,
                Width = 48,
            };
            image.Bind(ImageLoader.SourceProperty, new Binding(sourceBinding));
            Grid grid = new Grid()
            {
                Height = 48,
                Width = 48,
                Margin = new Thickness(12, 6)
            };
            grid.Children.Add(placeholder);
            grid.Children.Add(image);
            return grid;
        });
        return new DataGridTemplateColumn
        {
            CellTemplate = imgTemplate
        };
    }
}