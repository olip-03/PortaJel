using System.Globalization;
using Portajel.Connections.Database;

namespace Portajel.Structures.Converters;

public class SubtitleTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case PlaylistData playlist:
                string playlistDuration = playlist.Duration.TotalHours >= 1 
                    ? playlist.Duration.ToString(@"h\:mm\:ss")
                    : playlist.Duration.ToString(@"mm\:ss");
                string playlistCount = playlist.ChildCount > 1 ? "songs" : "song";
                return $"{playlistDuration} • {playlist.ChildCount} {playlistCount}";
            case SongData song:
                string songDuration = song.Duration.TotalHours >= 1 
                    ? song.Duration.ToString(@"h\:mm\:ss")
                    : song.Duration.ToString(@"mm\:ss");
                return $"{songDuration} • {song.ArtistNames}";
            case AlbumData album:
                return album.ArtistNames;
            case GenreData genre:
                string countText = genre.ChildCount > 1 ? "albums" : "album";
                return $"{genre.Duration} • {genre.ChildCount} {countText}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}