using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Portajel.Structures.ViewModels.Components;

public class PlaybackControlsViewModel: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ImageSource PlayPauseIcon
    {
        get => _playPauseIcon;
        set
        {
            if (_playPauseIcon != value)
            {
                _playPauseIcon = value;
                OnPropertyChanged();
            }
        }
    }
    private ImageSource _playPauseIcon = "media_play.png";
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}