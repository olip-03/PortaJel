using Microsoft.Maui.Controls;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Connections.Structs;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Functional
{
    public static class SaveHelper
    {
        private static string model = DeviceInfo.Current.Model;
        private static string manufacturer = DeviceInfo.Current.Manufacturer;
        private static string deviceName = DeviceInfo.Current.Name;
        public static async Task<IServerConnector> LoadData(IDbConnector database, string appDataDirectory)
        {
            Task<string?> r = SecureStorage.Default.GetAsync(GuidHelper.GetDeviceHash(model, manufacturer, deviceName));
            r.Wait();
            string? result = r.Result;

            if (result == null)
            {
                return new ServerConnector();
            }

            ServerConnectorSettings settings = new("", database, appDataDirectory);
            try
            {
                settings = new(result, database, appDataDirectory);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"LoadData(): {e.Message}");
                await SecureStorage.Default.SetAsync(GuidHelper.GetDeviceHash(model, manufacturer, deviceName), "");
                return new ServerConnector();
            }
            return settings.ServerConnector;
        }
        public static async Task<bool> SaveData(IServerConnector server)
        {
            try
            {
                var settings = new ServerConnectorSettings(server, server.Servers.ToArray());
                await SecureStorage.Default.SetAsync(GuidHelper.GetDeviceHash(model, manufacturer, deviceName), settings.ToJson());
            }
            catch (Exception e)
            {
                Trace.WriteLine($"SaveData(): {e.Message}");
                return false;
            }
            return true;
        }
        // Saves a blurhash to file and returns the path
        public static async Task<string> SaveBlurhash(SKBitmap blurhash)
        {
            string cacheDir = FileSystem.Current.CacheDirectory;
            string path = Path.Combine(cacheDir, $"blur/{blurhash}.png");
            try
            {
                using var image = SKImage.FromBitmap(blurhash);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100); // Format and quality
                using var stream = File.OpenWrite(path);
                data.SaveTo(stream);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"SaveData(): {e.Message}");
                return "";
            }
            return path;
        }
    }
}
