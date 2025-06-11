using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Portajel.Components
{
    public class MusicImage : SKCanvasView, IDisposable
    {
        private SKBitmap? _bitmap = null;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _bitmapLock = new object();
        private static readonly HttpClient _httpClient = new HttpClient();

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(MusicImage),
                defaultValue: null,
                propertyChanged: OnImageSourceChanged
            );

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(
                nameof(PlaceholderColor),
                typeof(Color),
                typeof(MusicImage),
                defaultValue: Colors.LightGray
            );

        public ImageSource? ImageSource
        {
            get => (ImageSource?)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MusicImage musicImage)
            {
                _ = musicImage.LoadImageAsync();
            }
        }

        public MusicImage()
        {
            PaintSurface += OnPaintSurface;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (ImageSource != null)
            {
                _ = LoadImageAsync();
            }
        }

        private async Task LoadImageAsync()
        {
            if (ImageSource == null)
            {
                ClearBitmap();
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                var bitmap = await Task.Run(() => LoadImageInternal(ImageSource, cancellationToken), cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    bitmap?.Dispose();
                    return;
                }

                lock (_bitmapLock)
                {
                    _bitmap?.Dispose();
                    _bitmap = bitmap;
                }

                InvalidateSurface();
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                ClearBitmap();
            }
        }

        private void ClearBitmap()
        {
            lock (_bitmapLock)
            {
                _bitmap?.Dispose();
                _bitmap = null;
            }
            InvalidateSurface();
        }

        private async Task<SKBitmap?> LoadImageInternal(ImageSource imageSource, CancellationToken cancellationToken)
        {
            Stream? stream = null;
            try
            {
                stream = await GetImageStreamAsync(imageSource, cancellationToken);
                if (stream == null) return null;

                cancellationToken.ThrowIfCancellationRequested();

                return SKBitmap.Decode(stream);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        private async Task<Stream?> GetImageStreamAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            return imageSource switch
            {
                FileImageSource fileSource => await GetFileStreamAsync(fileSource),
                UriImageSource uriSource => await GetUriStreamAsync(uriSource, cancellationToken),
                StreamImageSource streamSource => await streamSource.Stream(cancellationToken),
                _ => null
            };
        }

        private async Task<Stream?> GetFileStreamAsync(FileImageSource fileSource)
        {
            try
            {
                if (string.IsNullOrEmpty(fileSource.File)) return null;
                return await FileSystem.OpenAppPackageFileAsync(fileSource.File);
            }
            catch
            {
                return null;
            }
        }

        private async Task<Stream?> GetUriStreamAsync(UriImageSource uriSource, CancellationToken cancellationToken)
        {
            try
            {
                if (uriSource.Uri == null) return null;
                var response = await _httpClient.GetAsync(uriSource.Uri, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch
            {
                return null;
            }
        }

        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var size = Math.Min(e.Info.Width, e.Info.Height);
            var rect = new SKRect(0, 0, size, size);

            canvas.Clear(PlaceholderColor.ToSKColor());

            SKBitmap? currentBitmap;
            lock (_bitmapLock)
            {
                currentBitmap = _bitmap;
            }

            if (currentBitmap != null)
            {
                canvas.DrawBitmap(currentBitmap, rect);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            lock (_bitmapLock)
            {
                _bitmap?.Dispose();
                _bitmap = null;
            }
        }
    }

    public static class ColorExtensions
    {
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor(
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255),
                (byte)(color.Alpha * 255)
            );
        }
    }
}