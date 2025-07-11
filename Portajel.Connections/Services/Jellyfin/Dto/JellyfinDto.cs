namespace Portajel.Connections.Services.Jellyfin.Dto;

public class JfItemsDto
{
    public List<JfBaseItemDto> Items { get; set; } = new();
    public int TotalRecordCount { get; set; }
    public int StartIndex { get; set; }
}

public class JfBaseItemDto
{
    public string Name { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public Guid Id { get; set; } 
    public DateTime DateCreated { get; set; }
    public DateTime? PremiereDate { get; set; }
    public List<JfExternalUrl> ExternalUrls { get; set; } = new();
    public string? ChannelId { get; set; }
    public double? CommunityRating { get; set; }
    public long RunTimeTicks { get; set; }
    public int? ProductionYear { get; set; }
    public Dictionary<string, string> ProviderIds { get; set; } = new();
    public bool IsFolder { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ParentLogoItemId { get; set; } = string.Empty;
    public string ParentBackdropItemId { get; set; } = string.Empty;
    public List<string> ParentBackdropImageTags { get; set; } = new();
    public JfUserData UserData { get; set; } = new();
    public List<string> Artists { get; set; } = new();
    public List<JfArtistItem> ArtistItems { get; set; } = new();
    public string AlbumArtist { get; set; } = string.Empty;
    public List<JfArtistItem> AlbumArtists { get; set; } = new();
    public Dictionary<string, string> ImageTags { get; set; } = new();
    public List<string> BackdropImageTags { get; set; } = new();
    public string ParentLogoImageTag { get; set; } = string.Empty;
    public Dictionary<string, Dictionary<string, string>> ImageBlurHashes { get; set; } = new();
    public string LocationType { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public double? NormalizationGain { get; set; }
}

public class JfExternalUrl
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class JfUserData
{
    public long PlaybackPositionTicks { get; set; }
    public int PlayCount { get; set; }
    public bool IsFavorite { get; set; }
    public bool Played { get; set; }
    public string Key { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public DateTimeOffset LastPlayedDate { get; set; }
}

public class JfArtistItem
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}