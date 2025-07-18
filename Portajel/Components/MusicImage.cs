﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Blurhash;
using FFImageLoading.Cache;
using FFImageLoading.Maui;
using Jellyfin.Sdk.Generated.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Portajel.Components
{
    public class MusicImage : Grid, IDisposable
    {
        internal static IServiceProvider? ServiceProvider { get; set; } = IPlatformApplication.Current?.Services;
        private HttpClient? _client = default!;
        private readonly Random _random = new Random();
        private int _width => 64;
        private int _height => 64;
        
        private SKBitmap _sourceBitmap;
        private SKBitmap _blurhashBitmap;
        private string _lastBlurHash;

        private bool _imageLoading = true;
        private CancellationTokenSource _canTokenSource = new();

        private SKCanvasView _blurhashCanvas;
        private CachedImage _sourceImage;
        private Color _backgroundColor = Color.FromRgba(0, 0, 0, 255);
        
        public static readonly BindableProperty BlurHashProperty =
            BindableProperty.Create(
                nameof(BlurHash),
                typeof(string),
                typeof(MusicImage),
                defaultValue: string.Empty,
                propertyChanged: OnBlurHashChanged);

        public string BlurHash
        {
            get => (string)GetValue(BlurHashProperty);
            set => SetValue(BlurHashProperty, value);
        }
        
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(string),
                typeof(MusicImage),
                defaultValue: string.Empty,
                propertyChanged: OnSourceChanged);

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        
        public MusicImage()
        {
            _client = ServiceProvider.GetService<HttpClient>();
            _blurhashCanvas = new SKCanvasView()
            {
                ZIndex = 0
            };
            _blurhashCanvas.PaintSurface += OnPaintBlurhashSurface;
            
            _sourceImage = new CachedImage()
            {
                ZIndex = 1,
                CacheType = FFImageLoading.Cache.CacheType.Memory,
                RetryCount = 0,
                FadeAnimationForCachedImages = true,
                LoadingDelay = 500,
                LoadingPriority = FFImageLoading.Work.LoadingPriority.Lowest,
                HeightRequest = HeightRequest,
                WidthRequest = WidthRequest,
                Aspect = Aspect.AspectFill,
                BitmapOptimizations = true,
                DownsampleToViewSize = true,
                BackgroundColor = Colors.Transparent
            };
            
            Children.Add(_blurhashCanvas);
            Children.Add(_sourceImage);
            
            Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += (s, a) =>
            {
                UpdateColor();
            };
            UpdateColor();
        }

        private void UpdateColor()
        {
            var imageColor = Application.Current.Resources.TryGetValue(
                "BackgroundColor", 
                out object primaryColor
            );
            if (imageColor)
            {
                _backgroundColor = (Color)primaryColor;
            }
        }

        private static void OnBlurHashChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MusicImage musicImage && newValue is string blurHash)
            {
                musicImage._blurhashBitmap?.Dispose();
                musicImage._blurhashBitmap = null;
                musicImage._lastBlurHash = null;
                if (string.IsNullOrWhiteSpace(blurHash))
                {
                    musicImage._imageLoading = false;
                }
            }
        }
        private static async void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MusicImage musicImage)
            {
                musicImage._sourceImage.Source = null;
                if (oldValue is string oldSource && !string.IsNullOrEmpty(oldSource))
                {
                    await FFImageLoading.ImageService.Instance.InvalidateCacheEntryAsync(oldSource, CacheType.Memory);
                }

                if (newValue is string source && !string.IsNullOrEmpty(source))
                {
                    musicImage._sourceImage.Source = source;
                }
            }
        }
        
        private void OnPaintBlurhashSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info   = e.Info;

            var r = (byte)(_backgroundColor.Red   * 255);
            var g = (byte)(_backgroundColor.Green * 255);
            var b = (byte)(_backgroundColor.Blue  * 255);
            var a = (byte)(_backgroundColor.Alpha * 255);
            canvas.Clear(new SKColor(r, g, b, a));

            if (string.IsNullOrEmpty(BlurHash))
                return;

            try
            {
                if (_blurhashBitmap == null || _lastBlurHash != BlurHash)
                {
                    _blurhashBitmap?.Dispose();
                    _blurhashBitmap = CreateBlurhashBitmap();
                    _lastBlurHash  = BlurHash;
                }
                
                if (_blurhashBitmap != null)
                {
                    var destRect = new SKRect(0, 0, info.Width, info.Height);
                    using var blurPaint = new SKPaint
                    {
                        ImageFilter = SKImageFilter.CreateBlur(
                            sigmaX: 8, // horizontal blur radius
                            sigmaY: 8, // vertical blur radius
                            tileMode: SKShaderTileMode.Decal
                        )
                    };
                    canvas.DrawBitmap(_blurhashBitmap, destRect, blurPaint);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"MusicImage: Error decoding blurhash: {ex.Message}");
                canvas.Clear(new SKColor(255, 255, 255));

            }
        }

        private SKBitmap CreateBlurhashBitmap()
        {
            var pixels = new Pixel[_width, _height];
            Core.Decode(BlurHash, pixels);

            var bitmap = new SKBitmap(_width, _height, SKColorType.Bgra8888, SKAlphaType.Opaque);
            var pixelPtr = bitmap.GetPixels();
            unsafe
            {
                var pixelSpan = new Span<byte>(pixelPtr.ToPointer(), bitmap.ByteCount);
                int index = 0;

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var pixel = pixels[x, y];

                        pixelSpan[index++] = (byte)MathUtils.LinearTosRgb(pixel.Blue);
                        pixelSpan[index++] = (byte)MathUtils.LinearTosRgb(pixel.Green);
                        pixelSpan[index++] = (byte)MathUtils.LinearTosRgb(pixel.Red);
                        pixelSpan[index++] = 255;
                    }
                }
            }

            return bitmap;
        }

        public void Dispose()
        {
            _blurhashBitmap?.Dispose();
            _blurhashBitmap = null;
            _sourceBitmap?.Dispose();
            _sourceBitmap = null;
            if (_blurhashCanvas != null)
            {
                _blurhashCanvas.PaintSurface -= OnPaintBlurhashSurface;
                _blurhashCanvas = null;
            }
        }
    }
}