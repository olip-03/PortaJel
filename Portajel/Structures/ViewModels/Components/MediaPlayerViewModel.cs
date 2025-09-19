using Portajel.Connections.Database;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Portajel.Structures.ViewModels.Components;

public class MediaPlayerViewModel
{
    public int QueuePosition { get; set; } = 0;
    public ObservableCollection<SongData> Queue { get; set; } = new();
    public ObservableSongData Current { get; set; } = new ObservableSongData(SongData.Empty);
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