
using ExCSS;
using Portajel.Connections.Database;
using Portajel.Connections.Services;
using Portajel.Structures.ViewModels.Settings.Connections;

namespace Portajel.Pages.Views;

public partial class AlbumPage : ContentPage, IQueryAttributable
{
    private string url = default!;
    private Dictionary<string, AlbumData> _connectionProperties = new();

    private ViewConnectionViewModel _viewModel = new();

    public AlbumPage()
	{
		InitializeComponent();
        ApplyEdgeToEdgeLayout();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var objects = query;
        foreach (var item in objects)
        {
            try
            {
                AlbumData? itemValue = item.Value as AlbumData;
                BindingContext = itemValue;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        AlbumData? connectorProperty = new AlbumData();
        var test = _connectionProperties.TryGetValue("URL", out connectorProperty);
    }

    private void ApplyEdgeToEdgeLayout()
    {
#if ANDROID
        // Setting up the page to handle insets properly
        Microsoft.Maui.Handlers.PageHandler.Mapper.AppendToMapping("EdgeToEdge", (handler, page) =>
        {
            var nativeView = handler.PlatformView;
            if (nativeView is Android.Views.View androidView)
            {
                // Request layout changes when window insets change
                androidView.SetOnApplyWindowInsetsListener(new WindowInsetsListener(this));
            }
        });
#endif
    }

#if ANDROID
    private class WindowInsetsListener : Java.Lang.Object, Android.Views.View.IOnApplyWindowInsetsListener
    {
        private readonly MainPage _page;
        
        public WindowInsetsListener(MainPage page)
        {
            _page = page;
        }
        
        public Android.Views.WindowInsets OnApplyWindowInsets(Android.Views.View v, Android.Views.WindowInsets insets)
        {
            var systemBars = insets.GetInsetsIgnoringVisibility(Android.View.WindowInsets.Type.SystemBars());
            var displayCutout = insets.GetInsetsIgnoringVisibility(Android.View.WindowInsets.Type.DisplayCutout());
            
            // Apply insets to your UI elements
            Device.BeginInvokeOnMainThread(() =>
            {
                // For example, adjust padding of your main container
                _page.mainContainer.Padding = new Thickness(
                    displayCutout.Left,
                    systemBars.Top,
                    displayCutout.Right,
                    systemBars.Bottom
                );
                
                // For a FAB that should be above the navigation bar
                _page.floatingActionButton.Margin = new Thickness(
                    _page.floatingActionButton.Margin.Left,
                    _page.floatingActionButton.Margin.Top,
                    _page.floatingActionButton.Margin.Right,
                    systemBars.Bottom + 16 // 16dp additional padding
                );
            });
            
            // Return the insets so they can propagate to children
            return insets;
        }
    }
#endif

    // Handle scroll content properly with edge-to-edge
    private void SetupScrollViewForEdgeToEdge(CollectionView collectionView)
    {
        // In XAML, set padding and manage clipsToBounds
        collectionView.Margin = new Thickness(0, GetStatusBarHeight(), 0, GetNavigationBarHeight());

        // In MAUI, we need a platform-specific implementation for clipToPadding
#if ANDROID
        Microsoft.Maui.Handlers.CollectionViewHandler.Mapper.AppendToMapping("ClipToPadding", (handler, view) =>
        {
            if (handler.PlatformView is AndroidX.RecyclerView.Widget.RecyclerView recyclerView)
            {
                recyclerView.ClipToPadding = false;
            }
        });
#endif
    }

    private double GetStatusBarHeight()
    {
#if ANDROID
        var resources = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Resources;
        var resourceId = resources.GetIdentifier("status_bar_height", "dimen", "android");
        if (resourceId > 0)
        {
            return resources.GetDimensionPixelSize(resourceId) / Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Resources.DisplayMetrics.Density;
        }
#endif
        return 0;
    }

    private double GetNavigationBarHeight()
    {
#if ANDROID
        var resources = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Resources;
        var resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
        if (resourceId > 0)
        {
            return resources.GetDimensionPixelSize(resourceId) / Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Resources.DisplayMetrics.Density;
        }
#endif
        return 0;
    }
}