using Blurhash;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Jellyfin.Dto;
using SkiaSharp;

namespace Portajel.Connections.Structs
{
    public static class MusicItemImage
    {
        public static string GetImgSource(string serverUrl, JfBaseItemDto baseItem, JfImageType Type = JfImageType.Primary)
        {
            // https://media.oli.fm/Items/0008ba8bfd86aeb5856923c2e4b5c13a/Images/Primary?fillHeight=251&fillWidth=251&quality=96&tag=6c8386e6be4adb6e15d6e3b294fa1733
            string value = $"default_album.png";
            switch (baseItem.Type)
            {
                case "MusicAlbum":
                    value = $"default_album.png";
                    break;
                case "MusicArtist":
                    value = $"default_artist.png";
                    break;
                case "Audio":
                    // We need imgSrc from parent 
                    if (baseItem.ImageBlurHashes.ContainsKey("Primary"))
                    {
                        value = $"{serverUrl}/Items/{GuidString(baseItem.ParentId)}/Images/Primary";
                    }
                    else if (baseItem.ImageBlurHashes.ContainsKey("Backdrop"))
                    {
                        value = $"{serverUrl}/Items/{GuidString(baseItem.Id)}/Images/Backdrop/0";
                    }
                    break;
            }
            switch (Type)
            {
                case JfImageType.Primary:
                    if (baseItem.ImageTags.ContainsKey("Primary"))
                    {
                        value = $"{serverUrl}/Items/{GuidString(baseItem.Id)}/Images/Primary?tag={baseItem.ImageTags["Primary"]}";
                    }
                    else if (baseItem.ImageTags.ContainsKey("Backdrop"))
                    {
                        value = $"{serverUrl}Items/{GuidString(baseItem.Id)}/Images/Backdrop/0";
                    }
                    break;
                case JfImageType.Backdrop:
                    if (baseItem.ImageBlurHashes.ContainsKey("Backdrop"))
                    {
                        value = $"{serverUrl}/Items/{GuidString(baseItem.Id)}/Images/Backdrop/0";
                    }
                    else
                    {
                        value = "";
                    }
                    break;
                case JfImageType.Logo:
                    if (baseItem.ImageTags.ContainsKey("Logo"))
                    {
                        value = $"{serverUrl}/Items/{GuidString(baseItem.Id)}/Images/Logo?tag={baseItem.ImageTags["Logo"]}";
                    }
                    else
                    {
                        value = "";
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }

            return value;
        }

        public static string GetImgBlurhash(string serverUrl, JfBaseItemDto baseItem, JfImageType Type = JfImageType.Primary)
        {
            string value = $"";
            switch (baseItem.Type)
            {
                case "Audio":
                    // We need imgSrc from parent 
                    if (baseItem.ImageBlurHashes.ContainsKey("Primary"))
                    {
                        value = $"{baseItem.ImageBlurHashes["Primary"].First().Value}";
                    }
                    else if (baseItem.ImageBlurHashes.ContainsKey("Backdrop"))
                    {
                        value = $"{baseItem.ImageBlurHashes["Backdrop"].First().Value}";
                    }
                    break;
            }
            switch (Type)
            {
                case JfImageType.Primary:
                    if (baseItem.ImageBlurHashes.ContainsKey("Primary"))
                    {
                        var key = baseItem.ImageBlurHashes["Primary"];
                        value = baseItem.ImageBlurHashes["Primary"].First().Value;
                    }
                    break;
                case JfImageType.Backdrop:
                    if (baseItem.ImageBlurHashes.ContainsKey("Backdrop"))
                    {
                        value = baseItem.ImageBlurHashes["Backdrop"].First().Value;
                    }
                    break;
                case JfImageType.Logo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }
            return value;
        }
        
        private static string GuidString(string id)
        {
            return Guid.Parse(id).ToString("N");
        }
    }

    public enum MusicItemImageType
    {
        onDisk,
        url,
        base64,
    }
    public enum ImageBuilderImageType
    {
        Primary,
        Backdrop,
        Logo
    }
}