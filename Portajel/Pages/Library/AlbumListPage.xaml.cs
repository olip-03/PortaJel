using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages.Library;

namespace Portajel.Pages.Library;

public partial class AlbumListPage : ContentPage
{
    private AlbumListViewModel _vm;
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

        ImageService.Instance.SetPauseWork(false);
        var list = vlv;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}