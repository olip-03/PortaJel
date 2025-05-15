using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Functional;
using Portajel.Structures.ViewModels.Pages.Library;
using System.Diagnostics;

namespace Portajel.Pages.Library;

public partial class AlbumListPage : ContentPage
{
    private double scroll = 0;

    private ListHelper listHelper;
    private AlbumListViewModel _vm;
    private CancellationTokenSource CancellationTokenSource = new();

	public AlbumListPage(IDbConnector database)
	{
        IConfiguration imgConfig = new Configuration();
        imgConfig.DecodingMaxParallelTasks = 4;
        imgConfig.VerboseLogging = false;
        imgConfig.VerbosePerformanceLogging = false;
        imgConfig.HttpHeadersTimeout = 15;
        imgConfig.HttpReadTimeout = 15;
        ImageService.Instance.Initialize(imgConfig);

        listHelper = new(ImageService.Instance);

        _vm = new(database.GetDataConnectors()["Album"]);
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