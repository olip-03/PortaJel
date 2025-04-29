using Portajel.Connections.Data;
using Portajel.Connections.Structs;
using SkiaSharp;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Portajel.Components;

public class MusicListItem : ContentView
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

    public MusicListItemSize Size
    {
        get => (MusicListItemSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public MusicListItem()
    {
        BindingContext = this;
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        switch (Size)
        {
            case MusicListItemSize.GridLarge:
                Content = LargeGridLayout();
                break;
            default:
                Content = MediumLayout();
                break;
        }
    }

    private Grid MediumLayout()
    {
        // Create main grid
        Grid grid = new Grid();

        // Create transparent button with Z-index 0
        Button button = new Button
        {
            BackgroundColor = Colors.Transparent,
            ZIndex = 0
        };

        // Create horizontal layout for image and text
        HorizontalStackLayout horizontalLayout = new HorizontalStackLayout
        {
            ZIndex = 5,
            Margin = new Thickness(10),
            InputTransparent = true,
            Spacing = 10
        };

        // Create image with bindings
        Image image = new Image
        {
            BackgroundColor = Colors.LightBlue,
            Aspect = Aspect.AspectFill,
            WidthRequest = 64,
            HeightRequest = 64
        };
        image.SetBinding(Image.SourceProperty, new Binding(nameof(ImgBlurhashSource), source: this));

        // Create vertical stack for text
        VerticalStackLayout textStack = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Spacing = 2
        };

        // Create name label
        Label nameLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        nameLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        // Create artist label
        Label artistLabel = new Label
        {
            FontSize = 14,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        artistLabel.SetBinding(Label.TextProperty, new Binding(nameof(Subtitle), source: this));

        // Add labels to text stack
        textStack.Add(nameLabel);
        textStack.Add(artistLabel);

        // Add image and text stack to horizontal layout
        horizontalLayout.Add(image);
        horizontalLayout.Add(textStack);

        // Add elements to grid
        grid.Add(button);
        grid.Add(horizontalLayout);

        return grid;
    }

    private Grid LargeGridLayout()
    {
        // Create main grid
        Grid grid = new Grid();

        // Create transparent button with Z-index 0
        Button button = new Button
        {
            BackgroundColor = Colors.Transparent,
            ZIndex = 0
        };

        // Create stack layout with higher Z-index
        VerticalStackLayout stackLayout = new VerticalStackLayout
        {
            ZIndex = 5,
            Margin = new Thickness(10),
            MaximumWidthRequest = 210,
            InputTransparent = true
        };

        // Create image with bindings
        Image image = new Image
        {
            BackgroundColor = Colors.LightBlue,
            Aspect = Aspect.AspectFill,
            WidthRequest = 200,
            HeightRequest = 200
        };
        image.SetBinding(Image.SourceProperty, "Blurhash");

        // Create name label
        Label nameLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        nameLabel.SetBinding(Label.TextProperty, "Title");

        // Create artist label
        Label artistLabel = new Label
        {
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        artistLabel.SetBinding(Label.TextProperty, "Subtitle");

        // Add elements to stack layout
        stackLayout.Add(image);
        stackLayout.Add(nameLabel);
        stackLayout.Add(artistLabel);

        // Add elements to grid
        grid.Add(button);
        grid.Add(stackLayout);

        return grid;
    }
}

public class BlurhashToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string blurhash)
        {
            // Default dimensions if not specified
            int width = 64;
            int height = 64;

            if (parameter is string dimensions)
            {
                var parts = dimensions.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int h))
                {
                    width = w;
                    height = h;
                }
            }

            var bitmap = Blurhasher.Decode(blurhash, width, height);
            var image = SKImage.FromBitmap(bitmap);
            var data = image.Encode();

            return ImageSource.FromStream(() => data.AsStream());
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// Enum to define the size of the MusicListItem.
/// Small: Small size, for showing songs in an album or a playlist
/// Medium: Medium size, for showing music items in the library or search page
/// GridSmall: For 3x3 grids in the library or search page
/// GridLarge: For 2x2 grids in the library or search page
/// </summary>
public enum MusicListItemSize
{
    Small,
    Medium,
    GridSmall,
    GridLarge
}
