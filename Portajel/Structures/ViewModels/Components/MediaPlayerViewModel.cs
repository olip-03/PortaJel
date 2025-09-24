using Portajel.Connections.Database;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Portajel.Structures.ViewModels.Components;

public class MediaPlayerViewModel: INotifyPropertyChanged
{
    private ImageSource _playPauseIcon = "media_play.png";
    public ImageSource PlayPauseIcon
    {
        get => _playPauseIcon;
        set
        {
            if (_playPauseIcon != value)
            {
                _playPauseIcon = value;
                OnPropertyChanged(nameof(PlayPauseIcon));
            }
        }
    }
    
    private ImageSource _favButtonIcon = "favourite_empty.png";
    public ImageSource FavButtonIcon
    {
        get => _favButtonIcon;
        set
        {
            if (_favButtonIcon != value)
            {
                _favButtonIcon = value;
                OnPropertyChanged(nameof(FavButtonIcon));
            }
        }
    }
    
    public string Blurhash { get; set; } = string.Empty;
    public int QueuePosition { get; set; } = 0;
    public ObservableCollection<SongData> Queue { get; set; } = new();
    public ObservableSongData Current { get; set; } = new ObservableSongData(SongData.Empty);
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ObservableSongData : ObservableObject
{
    private readonly SongData _songData;
    public ObservableSongData(SongData songData) => this._songData = songData;

    public string Name
    {
        get => _songData.Name;
        set => SetProperty(_songData.Name, value, _songData, (u, n) => u.Name = n);
    }

    public string? ArtistNames
    {
        get => _songData.ArtistNames;
        set => SetProperty(_songData.ArtistNames, value, _songData, (u, n) => u.ArtistNames = n);
    }
}