using Newtonsoft.Json;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;

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
    public string Id { get; set; }
    public string CollectionType { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? PremiereDate { get; set; }
    public int? ProductionYear { get; set; }
    public List<JfExternalUrl> ExternalUrls { get; set; } = new();
    public string? ChannelId { get; set; }
    public double? CommunityRating { get; set; }
    public long RunTimeTicks { get; set; }
    public int ParentIndexNumber { get; set; }
    public string Path { get; set; } = String.Empty;
    public int ChildCount { get; set; }
    public int AlbumCount { get; set; }
    public List<JfGenreItem> GenreItems { get; set; } = new();
    public int IndexNumber { get; set; }
    public Dictionary<string, string> ProviderIds { get; set; } = new();
    public bool IsFolder { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ParentId { get; set; } 
    public string ParentLogoItemId { get; set; } = string.Empty;
    public string ParentBackdropItemId { get; set; } = string.Empty;
    public List<string> ParentBackdropImageTags { get; set; } = new();
    public string Overview { get; set; } = string.Empty;
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
    
    public static AlbumData AlbumBuilder(JfBaseItemDto baseItem, string server)
    {
        return new AlbumData()
        {
            ServerId = Guid.Parse(baseItem.Id),
            Id = Guid.Parse(baseItem.ParentId),
            GlobalId = BuildGlobalId(baseItem),
            ParentId = GuidHelper.GenerateNewGuidFromHash(Guid.Parse(baseItem.ParentId), server),
            Name = baseItem.Name ?? string.Empty,
            IsFavourite = baseItem.UserData.IsFavorite,
            PlayCount = baseItem.UserData.PlayCount,
            DateAdded = baseItem.DateCreated,
            DatePlayed = baseItem.UserData.LastPlayedDate,
            ServerAddress = server,
            ImgSource = MusicItemImage.GetImgSource(server, baseItem),
            ImgBlurhash = MusicItemImage.GetImgBlurhash(server, baseItem),
            ArtistNames = BuildArtistString(baseItem),
            ArtistIdsJson =
                JsonConvert.SerializeObject(baseItem.ArtistItems.Select(idPair => idPair.Id.ToString() ?? "")),
            GenresJson = GetGenreString(baseItem, server),
            PremiereDate = baseItem.PremiereDate,
            PremiereYear = baseItem.ProductionYear,
        };
    }

    public static ArtistData ArtistBuilder(JfBaseItemDto baseItem, string server)
    {
        return new ArtistData()
        {
            ServerId = Guid.Parse(baseItem.Id),
            Id = GuidHelper.GenerateNewGuidFromHash(Guid.Parse(baseItem.Id), server),
            GlobalId = BuildGlobalId(baseItem),
            ParentId = Guid.Parse(baseItem.ParentId),
            Name = baseItem.Name ?? string.Empty,
            IsFavourite = baseItem.UserData.IsFavorite,
            PlayCount = baseItem.UserData.PlayCount,
            DateAdded = baseItem.DateCreated,
            DatePlayed = baseItem.UserData.LastPlayedDate,
            ServerAddress = server,
            Description = baseItem.Overview,
            ImgSource = MusicItemImage.GetImgSource(server, baseItem),
            ImgBlurhash = MusicItemImage.GetImgBlurhash(server, baseItem),
            LogoImgSource = MusicItemImage.GetImgSource(server, baseItem, JfImageType.Logo),
            BackgroundImgSource = MusicItemImage.GetImgSource(server, baseItem, JfImageType.Backdrop),
            BackgroundImgBlurhashSource = MusicItemImage.GetImgBlurhash(server, baseItem, JfImageType.Backdrop),
        };
    }

    public static SongData SongBuilder(JfBaseItemDto baseItem, string server)
    {
        return new SongData()
        {
            ServerId = Guid.Parse(baseItem.Id),
            Id = GuidHelper.GenerateNewGuidFromHash(Guid.Parse(baseItem.Id), server),
            Duration = TimeSpan.FromTicks(baseItem.RunTimeTicks),
            DiskNumber = baseItem.ParentIndexNumber,
            IndexNumber = baseItem.IndexNumber,
            GlobalId = BuildGlobalId(baseItem),
            ParentId = Guid.Parse(baseItem.ParentId),
            Name = baseItem.Name ?? string.Empty,
            IsFavourite = baseItem.UserData.IsFavorite,
            PlayCount = baseItem.UserData.PlayCount,
            DateAdded = baseItem.DateCreated,
            DatePlayed = baseItem.UserData.LastPlayedDate,
            ServerAddress = server,
            ImgSource = MusicItemImage.GetImgSource(server, baseItem),
            ImgBlurhash = MusicItemImage.GetImgBlurhash(server, baseItem),
            ArtistNames = BuildArtistString(baseItem),
            ArtistIdsJson =
                JsonConvert.SerializeObject(baseItem.ArtistItems.Select(idPair => idPair.Id.ToString() ?? "")),
            StreamUrl = BuildStreamUrl(server, Guid.Parse(baseItem.Id)),
            PremiereDate = baseItem.PremiereDate,
            PremiereYear = baseItem.ProductionYear,
        };
    }

    public static PlaylistData PlaylistBuilder(JfBaseItemDto baseItem, string server)
    {
        return new PlaylistData()
        {
            ServerId = Guid.Parse(baseItem.Id),
            Id = GuidHelper.GenerateNewGuidFromHash(Guid.Parse(baseItem.Id), server),
            GlobalId = BuildGlobalId(baseItem),
            ParentId = Guid.Parse(baseItem.ParentId),
            Name = baseItem.Name ?? string.Empty,
            IsFavourite = baseItem.UserData.IsFavorite,
            PlayCount = baseItem.UserData.PlayCount,
            DateAdded = baseItem.DateCreated,
            DatePlayed = baseItem.UserData.LastPlayedDate,
            ServerAddress = server,
            ImgSource = MusicItemImage.GetImgSource(server, baseItem),
            ImgBlurhash = MusicItemImage.GetImgBlurhash(server, baseItem),
            Duration = TimeSpan.FromTicks(baseItem.RunTimeTicks),
            ChildCount = baseItem.ChildCount,
            Path = baseItem.Path ?? "",
        };
    }

    public static GenreData GenreBuilder(JfBaseItemDto baseItem, string server)
    {
        return new GenreData
        {
            ServerId = Guid.Parse(baseItem.Id),
            Id = GuidHelper.GenerateNewGuidFromHash(Guid.Parse(baseItem.Id), server),
            GlobalId = BuildGlobalId(baseItem),
            ParentId = Guid.TryParse(baseItem.ParentId, out var parentGuid) ? parentGuid : new Guid(),
            Name = baseItem.Name ?? string.Empty,
            IsFavourite = baseItem.UserData.IsFavorite,
            PlayCount = baseItem.UserData.PlayCount,
            // DateAdded  = IsValidDateTime(baseItem.DateCreated) ? baseItem.DateCreated : DateTime.MinValue,
            // DatePlayed  = IsValidDateTime(baseItem.UserData.LastPlayedDate) ? baseItem.UserData.LastPlayedDate : DateTime.MinValue,
            ServerAddress = server,
            ImgSource = MusicItemImage.GetImgSource(server, baseItem),
            ImgBlurhash = MusicItemImage.GetImgBlurhash(server, baseItem),
            ChildCount = baseItem.AlbumCount,
        };
    }

    private static Guid BuildGlobalId(JfBaseItemDto baseItem)
    {
        switch (baseItem.Type)
        {
            case "MusicAlbum":
                if (baseItem.ProviderIds != null && baseItem.ProviderIds.TryGetValue("MusicBrainzAlbum", out var gAlbumId))
                {
                    Guid.TryParse(gAlbumId.ToString(), out var albumId);
                    return albumId;
                }
                break;
            case "MusicArtist":
                if (baseItem.ProviderIds != null && baseItem.ProviderIds.TryGetValue("MusicBrainzArtist", out var gArtistId))
                {
                    Guid.TryParse(gArtistId.ToString(), out var albumId);
                    return albumId;
                }
                break;
            case "Audio":
                if (baseItem.ProviderIds != null && baseItem.ProviderIds.TryGetValue("MusicBrainzrRecording", out var gAudioId))
                {
                    Guid.TryParse(gAudioId.ToString(), out var albumId);
                    return albumId;
                }
                break;
        }
        return Guid.Empty; 
    }

    private static string BuildArtistString(JfBaseItemDto baseItem)
    {
        string artistNames = string.Empty;
        for (int i = 0; i < baseItem.ArtistItems.Count; i++)
        {
            artistNames += baseItem.ArtistItems[i].Name;
            if (i < baseItem.ArtistItems.Count - 1)
            {
                artistNames += ", ";
            }
        }
        return artistNames;
    }
    private static string GetGenreString(JfBaseItemDto baseItem, string server)
    {
        if (baseItem.GenreItems == null || !baseItem.GenreItems.Any())
            return JsonConvert.SerializeObject(new Dictionary<string, string>());
    
        var genreDict = baseItem.GenreItems.ToDictionary(
            genre => genre.Name, 
            genre => GuidHelper.GenerateNewGuidFromHash(Guid.Parse(genre.Id), server)
        );
    
        return JsonConvert.SerializeObject(genreDict);
    }
    private static bool IsValidDateTime(DateTime? dateTime)
    {
        if (!dateTime.HasValue) return false;
    
        var dt = dateTime.Value;
        return dt.Year >= 1 && dt.Year <= 9999 && 
               dt != DateTime.MinValue && dt != DateTime.MaxValue;
    }

    private static string BuildStreamUrl(string? server, Guid id)
    {
        return server + "/Audio/" + id + "/stream?static=true&audioCodec=adts&enableAutoStreamCopy=true&allowAudioStreamCopy=true&enableMpegtsM2TsMode=true&context=Static";
    }
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
    public DateTime? LastPlayedDate { get; set; }
}

public class JfArtistItem
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class JfGenreItem
{
    public string Name { get; set; }
    public string Id { get; set; }
}

public enum JfImageType
{
    Primary,
    Backdrop,
    Logo
}