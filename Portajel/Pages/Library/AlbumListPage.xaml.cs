using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages.Library;
using System.Diagnostics;

namespace Portajel.Pages.Library;

public partial class AlbumListPage : ContentPage
{
    private double scroll = 0;

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
        _ = Task.Run(() => listPoller(CancellationTokenSource.Token));
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        CancellationTokenSource.Cancel();
        base.OnNavigatedFrom(args);
    }

    private async Task<bool> listPoller(CancellationToken ct)
    {
        double previousScroll = 0;
        int stationaryCount = 0;
        bool isPaused = true; // Starting with paused as initialized in constructor

        while (true)
        {
            try
            {
                await Task.Delay(100, ct);

                // if its been the same twice in a row assume scrolling stalled 
                if (Math.Abs(scroll - previousScroll) < 0.1) // Tiny threshold for float comparison
                {
                    stationaryCount++;

                    // Wait for a sec tho just to ensure we've slowed down a touch
                    if (stationaryCount >= 3 && isPaused)
                    {
                        // Additional small delay to ensure we've really stopped
                        await Task.Delay(200, ct);

                        // then enable img loading
                        ImageService.Instance.SetPauseWork(false);
                        isPaused = false;
                        Trace.WriteLine("Scrolling stopped - resuming image loading :3");
                    }
                }
                else
                {
                    // Reset stationary counter when scroll position changes
                    stationaryCount = 0;

                    // otherwise if higher than 200 
                    if (scroll > 200 && !isPaused)
                    {
                        ImageService.Instance.SetPauseWork(true);
                        isPaused = true;
                        Trace.WriteLine("Fast scrolling detected - pausing image loading meow~");
                    }
                }

                previousScroll = scroll;
                ct.ThrowIfCancellationRequested();
            }
            catch (Exception)
            {
                Trace.WriteLine("Album list polling cancelled");
                break;
            }
        }
        return true;
    }

}