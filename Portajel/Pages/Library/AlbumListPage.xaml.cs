using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Structures.Functional;
using Portajel.Structures.ViewModels.Pages.Library;
using System.Diagnostics;

namespace Portajel.Pages.Library;

public partial class AlbumListPage : ContentPage
{
    private ListHelper listHelper = new(ImageService.Instance);
    private DatabaseBindViewModel _vm;
    private CancellationTokenSource CancellationTokenSource = new();
    private double scroll = 0;

    public AlbumListPage(IDbConnector database)
	{
        _vm = new(database.Connectors.Album);
        InitializeComponent();
        BindingContext = _vm;

        vlv.OnScrolled += Vlv_OnScrolled;

        ImageService.Instance.SetPauseWork(true);
    }

    private void Vlv_OnScrolled(object? sender, ScrolledEventArgs e)
    {
        scroll = e.ScrollY;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        CancellationTokenSource = new();
        _ = Task.Run(() => listHelper.listPoller(scroll, CancellationTokenSource.Token));
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        CancellationTokenSource.Cancel();
        base.OnNavigatedFrom(args);
    }
}