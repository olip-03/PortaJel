using FFImageLoading;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Functional;
using Portajel.Structures.ViewModels.Pages.Library;

namespace Portajel.Pages.Library;

public partial class PlaylistListPage : ContentPage
{
    private ListHelper listHelper = new(ImageService.Instance);
    private DatabaseBindViewModel _vm;
    private CancellationTokenSource CancellationTokenSource = new();
    private double scroll = 0;

    public PlaylistListPage(IDbConnector database)
	{
        _vm = new(database.Connectors.Playlist);
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