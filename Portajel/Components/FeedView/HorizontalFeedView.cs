using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using Portajel.Structures.Adaptor;
using SelectionMode = Microsoft.Maui.SelectionMode;

namespace Portajel.Components.FeedView;

public class HorizontalFeedView: VerticalStackLayout
{
    private IMediaFeed? _selectedFeed;
    public MediaFeedAdaptor? MusicAdapter { get; set; }
    private GridTemplateSelector _gridTemplateSelector;
    
    private BaseData[] feedItems = Array.Empty<BaseData>(); 
    StackLayout _layout = new();
    
    private readonly Label _title = new();

    public HorizontalFeedView(IMediaFeed feed)
    {
        _selectedFeed = feed;
        _gridTemplateSelector = new GridTemplateSelector();
        MusicAdapter = new MediaFeedAdaptor(feed);

        _title = new Label()
        {
            Margin = new Thickness(16, 0),
            Text = feed.Name,
            FontSize = 18
        };
        var vlv = new VirtualListView()
        {   
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            IsRefreshEnabled = true,
            SelectionMode = SelectionMode.None,
            Orientation = ListOrientation.Horizontal, 
            Adapter = MusicAdapter,
            ItemTemplateSelector = _gridTemplateSelector
        };
        var container = new Grid()
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill, 
            HeightRequest = 280,
            Children = { vlv }
        };
        HorizontalOptions = LayoutOptions.Fill;
        
        Children.Add(_title);
        Children.Add(container);
    }
}