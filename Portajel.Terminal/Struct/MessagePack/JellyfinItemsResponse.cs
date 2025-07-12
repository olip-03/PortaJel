using MessagePack;
using System;
using System.Collections.Generic;

namespace Portajel.Terminal.Struct.MessagePack
{
    [MessagePackObject]
    public class JellyfinItemsResponse
    {
        [Key(0)]
        public List<Item> Items { get; set; }

        [Key(1)]
        public int TotalRecordCount { get; set; }

        [Key(2)]
        public int StartIndex { get; set; }
    }

    [MessagePackObject]
    public class Item
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public Guid ServerId { get; set; }

        [Key(2)]
        public Guid Id { get; set; }

        [Key(3)]
        public DateTime PremiereDate { get; set; }

        [Key(4)]
        public string ChannelId { get; set; }

        [Key(5)]
        public long RunTimeTicks { get; set; }

        [Key(6)]
        public int ProductionYear { get; set; }

        [Key(7)]
        public bool IsFolder { get; set; }

        [Key(8)]
        public string Type { get; set; }

        [Key(9)]
        public string ParentLogoItemId { get; set; }

        [Key(10)]
        public string ParentBackdropItemId { get; set; }

        [Key(11)]
        public List<string> ParentBackdropImageTags { get; set; }

        [Key(12)]
        public List<string> Artists { get; set; }

        [Key(13)]
        public List<ArtistItem> ArtistItems { get; set; }

        [Key(14)]
        public ImageTags ImageTags { get; set; }

        [Key(15)]
        public ImageBlurhash ImageBlurHashes { get; set; }

        [Key(16)]
        public string ParentLogoImageTag { get; set; }

        [Key(17)]
        public string LocationType { get; set; }

        [Key(18)]
        public string MediaType { get; set; }

        [Key(19)]
        public double NormalizationGain { get; set; }
    }

    [MessagePackObject]
    public class ArtistItem
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public string Id { get; set; }
    }

    [MessagePackObject]
    public class ImageTags
    {
        [Key(0)]
        public string Primary { get; set; }

        [Key(1)]
        public string Backdrop { get; set; }
    }

    [MessagePackObject]
    public class ImageBlurhash
    {
        [Key(0)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}