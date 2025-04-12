using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Portajel.Components
{
    public class MusicImage : SKCanvasView, IDisposable
    {
        private SKShader? _shader = null;
        // Define bindable property
        public static readonly BindableProperty BlurHashProperty =
            BindableProperty.Create(
                nameof(BlurHash),
                typeof(string),
                typeof(MusicImage),
                defaultValue: string.Empty,
                propertyChanged: OnBlurHashChanged);

        // BlurHash property that uses the bindable property
        public string BlurHash
        {
            get => (string)GetValue(BlurHashProperty);
            set => SetValue(BlurHashProperty, value);
        }

        private int _width => WidthRequest >= 1 ? (int)WidthRequest * 3 : 1;
        private int _height => HeightRequest >= 1 ? (int)HeightRequest * 3 : 1;

        // Property to set the BlurHash
        private static void OnBlurHashChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MusicImage musicImage && newValue is string blurHash)
            {
                musicImage.CreateShaderFromBlurHash();
                musicImage.InvalidateSurface();
            }
        }

        // Set the dimensions for the generated image
        public void SetSize(int width, int height)
        {
            if (!string.IsNullOrEmpty(BlurHash))
            {
                CreateShaderFromBlurHash();
                InvalidateSurface();
            }
        }

        public MusicImage()
        {
            PaintSurface += OnPaintSurface;
        }

        protected override void OnParentSet()
        { 
            base.OnParentSet();
            if (!string.IsNullOrEmpty(BlurHash))
            {
                CreateShaderFromBlurHash();
            }
        }

        private void CreateShaderFromBlurHash()
        {
            if (string.IsNullOrEmpty(BlurHash))
                return;

            // Decode the BlurHash
            var decoded = BlurHashDecode(BlurHash);

            // Create a bitmap from the decoded data
            using var bitmap = GenerateBitmapFromBlurHash(decoded, _width, _height);

            // Create shader from bitmap
            _shader?.Dispose();
            _shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
        }

        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            if (_shader != null)
            {
                using var paint = new SKPaint
                {
                    Shader = _shader
                };

                // Draw a rectangle covering the whole canvas
                canvas.DrawRect(0, 0, e.Info.Width, e.Info.Height, paint);
            }
        }

        #region BlurHash Implementation

        // BlurHash implementation converted from TypeScript to C#
        private static readonly string Base83Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#$%*+,-.:;=?@[]^_{|}~";

        private static double SRGBToLinear(int value)
        {
            double v = value / 255.0;
            if (v <= 0.04045)
                return v / 12.92;
            else
                return Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        private static int LinearToSRGB(double value)
        {
            double v = Math.Max(0, Math.Min(1, value));
            if (v <= 0.0031308)
                return (int)(v * 12.92 * 255 + 0.5);
            else
                return (int)((1.055 * Math.Pow(v, 1 / 2.4) - 0.055) * 255 + 0.5);
        }

        private static double SignPow(double value, double exp)
        {
            return Math.Sign(value) * Math.Pow(Math.Abs(value), exp);
        }

        private static int Decode83(string str, int start, int count)
        {
            int value = 0;
            for (int i = start; i < start + count; i++)
            {
                if (i >= str.Length)
                    break;

                int digit = Base83Chars.IndexOf(str[i]);
                if (digit == -1)
                    continue;

                value = value * 83 + digit;
            }
            return value;
        }

        private static double[] DecodeDC(int value)
        {
            int intR = value >> 16;
            int intG = (value >> 8) & 255;
            int intB = value & 255;
            return new double[] { SRGBToLinear(intR), SRGBToLinear(intG), SRGBToLinear(intB) };
        }

        private static double[] DecodeAC(int value, double maximumValue)
        {
            int quantR = (int)Math.Floor(value / (19.0 * 19.0));
            int quantG = (int)Math.Floor(value / 19.0) % 19;
            int quantB = value % 19;

            return new double[]
            {
                SignPow((quantR - 9) / 9.0, 2.0) * maximumValue,
                SignPow((quantG - 9) / 9.0, 2.0) * maximumValue,
                SignPow((quantB - 9) / 9.0, 2.0) * maximumValue
            };
        }

        private class BlurHashResult
        {
            public int NumX { get; set; }
            public int NumY { get; set; }
            public double[][] Colors { get; set; } = Array.Empty<double[]>();
        }

        private static BlurHashResult BlurHashDecode(string blurHash, double punch = 1.0)
        {
            if (string.IsNullOrEmpty(blurHash) || blurHash.Length < 6)
            {
                return new BlurHashResult
                {
                    NumX = 0,
                    NumY = 0,
                    Colors = Array.Empty<double[]>()
                };
            }

            int sizeFlag = Decode83(blurHash, 0, 1);
            int numY = (int)Math.Floor(sizeFlag / 9.0) + 1;
            int numX = (sizeFlag % 9) + 1;

            int quantisedMaximumValue = Decode83(blurHash, 1, 1);
            double maximumValue = (quantisedMaximumValue + 1) / 166.0;

            int numColors = numX * numY;
            var colors = new double[numColors][];

            for (int i = 0; i < numColors; i++)
            {
                if (i == 0)
                {
                    int value = Decode83(blurHash, 2, 4);
                    colors[i] = DecodeDC(value);
                }
                else
                {
                    int value = Decode83(blurHash, 4 + i * 2, 2);
                    colors[i] = DecodeAC(value, maximumValue * punch);
                }
            }

            return new BlurHashResult
            {
                NumX = numX,
                NumY = numY,
                Colors = colors
            };
        }

        private static SKBitmap GenerateBitmapFromBlurHash(BlurHashResult decoded, int width, int height)
        {
            // Create an image info and bitmap
            var info = new SKImageInfo(width, height);
            var bitmap = new SKBitmap(info);

            if (decoded.NumX == 0 || decoded.NumY == 0 || decoded.Colors.Length == 0)
                return bitmap;

            // Create a canvas to draw on the bitmap
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear();

            using var paint = new SKPaint();

            // Process each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double r = 0, g = 0, b = 0;

                    for (int j = 0; j < decoded.NumY; j++)
                    {
                        for (int i = 0; i < decoded.NumX; i++)
                        {
                            double basis = Math.Cos(Math.PI * x * i / width) *
                                          Math.Cos(Math.PI * y * j / height);
                            int colorIndex = i + j * decoded.NumX;

                            if (colorIndex < decoded.Colors.Length)
                            {
                                var color = decoded.Colors[colorIndex];
                                if (color.Length >= 3)
                                {
                                    r += color[0] * basis;
                                    g += color[1] * basis;
                                    b += color[2] * basis;
                                }
                            }
                        }
                    }

                    // Convert linear RGB to sRGB
                    byte pixelR = (byte)LinearToSRGB(r);
                    byte pixelG = (byte)LinearToSRGB(g);
                    byte pixelB = (byte)LinearToSRGB(b);

                    // Draw a 1x1 rectangle at this position
                    paint.Color = new SKColor(pixelR, pixelG, pixelB);
                    canvas.DrawRect(x, y, 1, 1, paint);
                }
            }

            return bitmap;
        }

        public void Dispose()
        {
            _shader?.Dispose();
            _shader = null;
        }

        #endregion
    }
}
