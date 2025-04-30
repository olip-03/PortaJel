namespace Portajel.Components;

public partial class MusicListMedium : ContentView
{
    public static readonly BindableProperty ImgBlurhashSourceProperty =
    BindableProperty.Create(nameof(ImgBlurhashSource), typeof(string), typeof(MusicListItem), default(string));
    public string ImgBlurhashSource
    {
        get => (string)GetValue(ImgBlurhashSourceProperty);
        set => SetValue(ImgBlurhashSourceProperty, value);
    }

    // Image Source bindable property
    public static readonly BindableProperty ImgSourceProperty =
        BindableProperty.Create(nameof(ImgSource), typeof(string), typeof(MusicListItem), default(string));

    public string ImgSource
    {
        get => (string)GetValue(ImgSourceProperty);
        set => SetValue(ImgSourceProperty, value);
    }

    // Title bindable property
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MusicListItem), default(string));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // Subtitle bindable property
    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(MusicListItem), default(string));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    // Size bindable property
    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(MusicListItemSize), typeof(MusicListItem), default(MusicListItemSize));
    public MusicListMedium()
	{
		InitializeComponent();
        BindingContext = this;
	}
}