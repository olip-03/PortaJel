using Blurhash;
using SkiaSharp;
using System;
using System.Linq;

namespace Portajel.Connections.Structs
{
    public static class Blurhasher
    {
        /// <summary>
        /// Decodes a Blurhash string into a <c>SKBitmap</c>
        /// </summary>
        /// <param name="blurhash">The blurhash string to decode</param>
        /// <param name="outputWidth">The desired width of the output in pixels</param>
        /// <param name="outputHeight">The desired height of the output in pixels</param>
        /// <param name="punch">A value that affects the contrast of the decoded image. 1 means normal, smaller values will make the effect more subtle, and larger values will make it stronger.</param>
        /// <returns>The decoded preview</returns>
        public static SKBitmap Decode(string blurhash, int outputWidth, int outputHeight, double punch = 1.0)
        {
            if (string.IsNullOrEmpty(blurhash))
            {
                return new SKBitmap(outputWidth, outputHeight, SKColorType.Bgra8888, SKAlphaType.Opaque);
            }
            var pixelData = new Pixel[outputWidth, outputHeight];
            Core.Decode(blurhash, pixelData, punch);
            return ConvertToBitmap(pixelData);
        }

        /// <summary>
        /// Converts the library-independent representation of pixels into a bitmap
        /// </summary>
        /// <param name="pixelData">The library-independent representation of the image</param>
        /// <returns>A <c>SKBitmap</c> in Bgra8888 representation</returns>
        public static SKBitmap ConvertToBitmap(Blurhash.Pixel[,] pixelData)
        {
            var width = pixelData.GetLength(0);
            var height = pixelData.GetLength(1);

            var data = Enumerable.Range(0, height)
                .SelectMany(y => Enumerable.Range(0, width).Select(x => (x, y)))
                .Select(tuple => pixelData[tuple.x, tuple.y])
                .SelectMany(pixel => new byte[]
                {
                    (byte) MathUtils.LinearTosRgb(pixel.Blue), (byte) MathUtils.LinearTosRgb(pixel.Green),
                    (byte) MathUtils.LinearTosRgb(pixel.Red), 255
                })
                .ToArray();

            SKBitmap bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque);

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    bitmap.SetPixels((IntPtr)ptr);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Encodes a <c>SKBitmap</c> into a Blurhash string
        /// </summary>
        /// <param name="image">The bitmap to encode</param>
        /// <param name="componentsX">The number of components used on the X-Axis for the DCT</param>
        /// <param name="componentsY">The number of components used on the Y-Axis for the DCT</param>
        /// <returns>The resulting Blurhash string</returns>
        public static string Encode(SKBitmap image, int componentsX, int componentsY)
        {
            return Core.Encode(ConvertBitmap(image), componentsX, componentsY);
        }

        /// <summary>
        /// Converts the given bitmap to the library-independent representation used within the Blurhash-core
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to encode</param>
        public static unsafe Pixel[,] ConvertBitmap(SKBitmap sourceBitmap)
        {
            SKPixmap pixmap = sourceBitmap.PeekPixels();
            byte* bmpPtr = (byte*)pixmap.GetPixels().ToPointer();
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;

            var result = new Pixel[width, height];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    byte red, green, blue, alpha;
                    if (sourceBitmap.ColorType == SKColorType.Rgba8888)
                    {
                        // SKColorType.Rgba8888 is used by iOS and Android
                        red = *bmpPtr++;
                        green = *bmpPtr++;
                        blue = *bmpPtr++;
                        alpha = *bmpPtr++;

                        result[col, row].Blue = MathUtils.SRgbToLinear(blue);
                        result[col, row].Green = MathUtils.SRgbToLinear(green);
                        result[col, row].Red = MathUtils.SRgbToLinear(red);
                    }
                    else if (sourceBitmap.ColorType == SKColorType.Bgra8888)
                    {
                        // UWP uses SKColorType.Bgra8888
                        blue = *bmpPtr++;
                        green = *bmpPtr++;
                        red = *bmpPtr++;
                        alpha = *bmpPtr++;

                        result[col, row].Blue = MathUtils.SRgbToLinear(blue);
                        result[col, row].Green = MathUtils.SRgbToLinear(green);
                        result[col, row].Red = MathUtils.SRgbToLinear(red);
                    }
                    else
                    {
                        throw new ArgumentException($"ColorType {sourceBitmap.ColorType} is not supported");
                    }
                }
            }

            return result;
        }
    }
}
