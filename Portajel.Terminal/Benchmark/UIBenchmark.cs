using System.Buffers;
using BenchmarkDotNet.Attributes;
using CommandLine;
using Microsoft.Kiota.Abstractions.Extensions;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Services.Jellyfin;
using SkiaSharp;
using Blurhash;

namespace Portajel.Terminal.Benchmark;

[MemoryDiagnoser(false)]
public class UIBenchmark
{
    private DatabaseConnector _database = Program.Database;
    private ServerConnector _server = Program.Server;
    private HttpClient httpClient = new HttpClient();

    private AlbumData _albumData;

    private string filePath = "/home/oli/Downloads/Skia/test.png";
    
    [GlobalSetup]
    public void Setup()
    {
        Program.InitializeDatabase();  
        _database = Program.Database!;

        httpClient.DefaultRequestHeaders.Add("accept", "application/json"); 
        httpClient.DefaultRequestHeaders.Add("Authorization", "Mediabrowser Token=\"d3aa895e30534bb88df149f762409f46\""); 
        
        JellyfinServerConnector jf = new(
            _database,
            "https://media.oli.fm",
            "local",
            "test1234",
            "PortaJel-Benchy",
            "0.0.1",
            "Benchy",
            "Benchy",
            Program.AppDataPath);
        _server.AddServer(jf);
        var authTask = _server.AuthenticateAsync();
        authTask.Wait();
        
        Random r = new Random();
        while (true)
        {
            int rInt = r.Next(0, 1000);
            var task = _server.Servers.First().DataConnectors["Album"].GetAllAsync(limit: 1, startIndex: rInt);
            task.Wait();
            _albumData = task.Result.First().Cast<AlbumData>();
            if (!String.IsNullOrEmpty(_albumData.ImgBlurhash))
            {
                break;
            }
        }    
    }

    [Benchmark]
    public SKBitmap GetBlurhashBitmap()
    {
        var pixels = new Pixel[64, 64];
        Core.Decode(_albumData.ImgBlurhash, pixels);
    
        // Create SKBitmap with 64x64 dimensions
        var bitmap = new SKBitmap(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul);
    
        var pixelPtr = bitmap.GetPixels();
        unsafe
        {
            var pixelSpan = new Span<byte>(pixelPtr.ToPointer(), bitmap.ByteCount);
            int index = 0;
        
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    var pixel = pixels[x, y];
                
                    pixelSpan[index++] = (byte)(pixel.Red * 255);
                    pixelSpan[index++] = (byte)(pixel.Green * 255);
                    pixelSpan[index++] = (byte)(pixel.Blue * 255);
                    pixelSpan[index++] = 255; // Alpha
                }
            }
        }
        return bitmap;
    }
    
    [Benchmark]
    public async Task DowloadImage()
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, _albumData.ImgSource);
        using var message = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        var stream = await message.Content.ReadAsByteArrayAsync();
    }

        [Benchmark]
        public async Task<SKBitmap?> DownloadImageArrayPool()
        {
            SKBitmap? skBitmap = null;
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, _albumData.ImgSource);
            using var message = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            
            using var stream = message.Content.ReadAsStream();
            byte[] buffer = new byte[4096];
            while (await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false) > 0)
            {
                // TODO: Process data chunked
                skBitmap = SKBitmap.Decode(buffer.AsSpan(0, 4096));
            }
            
            // using var stream = message.Content.ReadAsStream();
            // if (!message.Content.Headers.ContentLength.HasValue)
            //     throw new NotSupportedException();
            // var buffer = ArrayPool<byte>.Shared.Rent((int)message.Content.Headers.ContentLength);
            // var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            // try
            // {
            //     foreach(var b in buffer)
            //     {
            //         skBitmap = SKBitmap.Decode(buffer.AsSpan(0, bytesRead));
            //     }
            // }
            // catch (Exception ex)
            // {
            //     
            // }
            // finally
            // {
            //     ArrayPool<byte>.Shared.Return(buffer);
            // }
            return skBitmap;
        }

    [Benchmark]
    public async Task DownloadAndSave()
    {
        var bitmap = await DownloadImageArrayPool();
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(filePath);
        data.SaveTo(stream);
    }
    
    public void SaveBitmapToFile(SKBitmap bitmap, string filePath, 
        SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality);
        using var stream = File.OpenWrite(filePath);
    
        data.SaveTo(stream);
    }
}