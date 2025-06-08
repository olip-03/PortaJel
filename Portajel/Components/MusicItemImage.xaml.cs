using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Components;

public partial class MusicItemImage : ContentView
{
    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(MusicItemImage),
            null);
    public static readonly BindableProperty BackgroundSourceProperty =
        BindableProperty.Create(
            nameof(BackgroundSource),
            typeof(ImageSource),
            typeof(MusicItemImage),
            null);
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(MusicItemImage),
            new CornerRadius(8));
    public static readonly BindableProperty ImageSizeProperty = 
        BindableProperty.Create(
            nameof(ImageSize),
            typeof(int),
            typeof(int),
            64);

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    public ImageSource BackgroundSource
    {
        get => (ImageSource)GetValue(BackgroundSourceProperty);
        set => SetValue(BackgroundSourceProperty, value);
    }

    public int ImageSize
    {
        get => (int)GetValue(ImageSizeProperty);
        set => SetValue(ImageSizeProperty, value);
    }
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public MusicItemImage()
    {
        InitializeComponent();
    }
}