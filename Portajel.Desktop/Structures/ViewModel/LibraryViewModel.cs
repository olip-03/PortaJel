using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using DynamicData;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Structs;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel
{
    private IDbItemConnector _dbItemConnection;
    public IDbItemConnector DbItemConnection { get => _dbItemConnection; }
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public ReactiveCommand<Unit, Unit> GoNext { get; }
    public ReactiveCommand<Unit, Unit> GoPrevious { get; }

    public ObservableCollection<BaseData> Items { get; } = new();

    public ObservableCollection<DataGridColumn> DataGridColumns { get; } = new();

    public LibraryViewModel(IScreen screen) => HostScreen = screen;

    public LibraryViewModel(IScreen screen, IDbItemConnector dbItemConnection)
    {
        HostScreen = screen;
        _dbItemConnection = dbItemConnection ?? throw new ArgumentNullException(nameof(dbItemConnection));
        
        GenerateColumns(_dbItemConnection.MediaType);
        _ = Task.Run(Load);
    }

    private void GenerateColumns(MediaTypes mediaType)
    {
        DataGridColumns.Clear();

        switch (mediaType)
        {
            case MediaTypes.Album:
                DataGridColumns.Add(CreateTextColumn("Name", "Name"));
                DataGridColumns.Add(CreateTextColumn("Artists", "ArtistNames"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaTypes.Artist:
                DataGridColumns.Add(CreateTextColumn("Name", "Name"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaTypes.Song:
                DataGridColumns.Add(CreateTextColumn("Name", "Name"));
                DataGridColumns.Add(CreateTextColumn("Artist", "Artist"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
            case MediaTypes.Genre:
                DataGridColumns.Add(CreateTextColumn("Name", "Name"));
                DataGridColumns.Add(CreateTextColumn("ArtistNames", "ArtistNames"));
                DataGridColumns.Add(CreateTextColumn("Added On", "DateAdded"));
                break;
        }
    }

    private DataGridTextColumn CreateTextColumn(string header, string bindingPath)
    {
        return new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(bindingPath),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star) 
        };
    }

    private Task<bool> Load()
    {
        try
        {
            var items = _dbItemConnection.GetAll(limit: 50);
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                Items.AddRange(items);
            });
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return Task.FromResult(false);
        }
    }
}