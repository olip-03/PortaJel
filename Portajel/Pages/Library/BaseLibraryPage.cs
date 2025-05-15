using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Functional;
using Portajel.Structures.ViewModels.Pages.Library;
namespace Portajel.Pages.Library
{
    public partial class BaseLibraryPage : ContentPage
    {
        private ListHelper listHelper;
        private AlbumListViewModel _vm;
        public void InitializeLibrary(ContentPage page, IDbConnector database, )
        {
            var imgConfig = new Configuration();
            imgConfig.DecodingMaxParallelTasks = 4;
            imgConfig.VerboseLogging = false;
            imgConfig.VerbosePerformanceLogging = false;
            imgConfig.HttpHeadersTimeout = 15;
            imgConfig.HttpReadTimeout = 15;
            ImageService.Instance.Initialize(imgConfig);
            listHelper = new(ImageService.Instance);
            _vm = new(GetDataConnectors()[MediaCapabilities.Artist]);
            page.BindingContext = _vm;

            ImageService.Instance.SetPauseWork(false);
        }
    }
}
