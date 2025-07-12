using Portajel.Connections.Database;
using Portajel.Connections.Structs;

namespace Portajel.Structures.ViewModels.Library;

public class MusicCellViewModel
{
    public string Name { get; set; } = string.Empty;
    public string ImgBlurhash { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;

    public MusicCellViewModel()
    {
        
    }

    public MusicCellViewModel(BaseData baseItem)
    {
        Name = baseItem.Name;
        ImgBlurhash = baseItem.ImgBlurhash;
        switch (baseItem)
        {
            case AlbumData album:
                Subtitle = album.ArtistNames;
                break;
            case ArtistData artist:
                break;
            case PlaylistData playlist:
                Subtitle = $"{playlist.Duration} • {playlist.ChildCount} songs";
                break;
            case SongData song:
                Subtitle = $"{song.Duration} • {song.ArtistNames}";
                break;
            case GenreData genre:
                Subtitle = $"{genre.Duration} • {genre.ChildCount} albums";
                break;
        }
    }
}