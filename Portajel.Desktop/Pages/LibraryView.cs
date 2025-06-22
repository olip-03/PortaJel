using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;

namespace Portajel.Desktop.Pages;

public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
{
    private IDbConnector _db = Ioc.Default.GetService<IDbConnector>();

    private DataGrid _dataGrid;
    
    bool _initialized = false;
    private int _prevItemCount = -1;
    public LibraryView()
    {
        this.WhenActivated(disposables =>
        {
            _dataGrid = this.FindControl<DataGrid>("DataGrid");
            _dataGrid.SizeChanged += OnSizeChanged;
            _dataGrid.LayoutUpdated += DataGridOnLayoutUpdated; 
            _dataGrid.Columns.Clear();
            _dataGrid.Columns.AddRange(ViewModel.DataGridColumns);
            Load();
        });
        AvaloniaXamlLoader.Load(this);
    }
    
    private async void Load()
    {
        int count = GetItemEstimate(_dataGrid.Bounds.Bottom) - 1;
        var items = ViewModel.DbItemConnection.GetAll(limit: count);
        ViewModel.Items.AddRange(items);
    }

    private void DataGridOnLayoutUpdated(object? sender, EventArgs e)
    {
        
    }

    private int GetItemEstimate(double gridHeight)
    {
        int rowHeight = 33;
        int headerHeight = 32;
        return (int)Math.Floor((gridHeight - headerHeight) / rowHeight);
    }
    
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
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
        _prevItemCount = itemCount;
    }
}