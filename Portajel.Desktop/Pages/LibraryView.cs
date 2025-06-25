using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Portajel.Connections.Database;
using Portajel.Desktop.Structures.Services;
using Portajel.Desktop.Structures.ViewModel;
using Portajel.Desktop.Structures.ViewModel.Music;
using ReactiveUI;

namespace Portajel.Desktop.Pages;

public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
{
    private readonly LibraryViewCache? _vc = Ioc.Default.GetService<LibraryViewCache>();

    private DataGrid? _dataGrid;
    
    private int _prevItemCount = -1;
    public LibraryView()
    {
        this.WhenActivated(disposables =>
        {
            _dataGrid = this.FindControl<DataGrid>("DataGrid");
            if (_dataGrid == null) return; 
            if (ViewModel == null) return; 
            _dataGrid.SizeChanged += OnSizeChanged;
            _dataGrid.Columns.Clear();
            _dataGrid.Columns.AddRange(ViewModel.DataGridColumns);
            Load();
        });
        AvaloniaXamlLoader.Load(this);
    }
    private void Load()
    {
        if (_dataGrid == null) return; 
        if (ViewModel == null) return; 
        if (_vc == null) return; 
        _prevItemCount = GetItemEstimate(_dataGrid.Bounds.Bottom);
        if (_vc.Initialized(ViewModel.DbItemConnection.MediaType))
        {
            var cachedItems = _vc.GetMediaType(ViewModel.DbItemConnection.MediaType).ToList();
            if (_prevItemCount > cachedItems.Count)
            {   // Expanding - need more items than cached
                int diff = _prevItemCount - cachedItems.Count;
                var additionalItems = ViewModel.DbItemConnection.GetAll(limit: diff, startIndex: cachedItems.Count);
                cachedItems.AddRange(additionalItems);
            
                // Update cache with expanded data
                _vc.StoreMediaType(ViewModel.DbItemConnection.MediaType, cachedItems);
                ViewModel.Items.AddRange(cachedItems);
            }
            else
            {   // Shrinking or exact match - take only what we need
                var itemsToDisplay = cachedItems.Take(_prevItemCount).ToList();
                ViewModel.Items.AddRange(itemsToDisplay);
            }
        }
        else
        {
            var items = ViewModel.DbItemConnection.GetAll(limit: _prevItemCount);
            ViewModel.Items.AddRange(items);
            _vc.StoreMediaType(ViewModel.DbItemConnection.MediaType, items);
        }
        
        int totalItems = ViewModel.DbItemConnection.GetTotalCount();
        if (totalItems > _prevItemCount)
        {
            ViewModel.MaxPageNumber = totalItems / _prevItemCount;
        }
    }
    /// <summary>
    /// Tells us how many items can fit inside a grid of a provided size
    /// </summary>
    /// <param name="gridHeight">The height of the DataGridView</param>
    /// <returns>Total amount of items that can fit</returns>
    private int GetItemEstimate(double gridHeight, int rowHeight = 58, int headerHeight = 32, int safeRoom = 1)
    {
        return (int)Math.Floor((gridHeight - headerHeight) / rowHeight) - safeRoom;
    }
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (ViewModel == null) return; 
        int itemCount = GetItemEstimate(e.NewSize.Height);
        if (_prevItemCount == -1)
        { // Init if not set
            _prevItemCount = itemCount;
        }
        if (_prevItemCount < itemCount)
        {   // Expanding
            int diff = itemCount - _prevItemCount;
            ViewModel.Items.AddRange(ViewModel.DbItemConnection.GetAll(limit: diff, startIndex: _prevItemCount));
        }
        else
        {   // Shrinking
            for (int i = ViewModel.Items.Count - 1; i >= itemCount; i--)
            {
                ViewModel.Items.RemoveAt(i);
            }
        }
        
        // Update max item count 
        int totalItems = ViewModel.DbItemConnection.GetTotalCount();
        if (totalItems > itemCount && itemCount > 0)
        {
            ViewModel.MaxPageNumber = totalItems / itemCount;
        }
        _prevItemCount = itemCount;
    }
    private void NumericUpDown_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (ViewModel == null) return; 
        if (_dataGrid == null) return; 

        if (_prevItemCount == -1)
        { // Init if not set
            _prevItemCount = GetItemEstimate(_dataGrid.Bounds.Bottom) - 1;;
        }
        
        int startFrom = _prevItemCount * ((int)e.NewValue.Value - 1);
        ViewModel.Items.Clear();
        ViewModel.Items.AddRange(ViewModel.DbItemConnection.GetAll(limit: _prevItemCount, startIndex: startFrom));
    }

    private void DataGrid_OnCellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        switch (e.Cell.DataContext)
        {
            case AlbumData album:
                switch (e.Column.Header)
                {
                    case "Name":
                        Program.Router.Navigate.Execute(new AlbumViewModel(ViewModel.HostScreen, album));
                        break;
                    case "Artists":
                        Trace.WriteLine("Navigate to Artist " + e.Cell.DataContext);
                        break;
                }
                break;
            case ArtistData:
                break;
            case SongData:
                break;
            case GenreData:
                break;
        }

    }
}