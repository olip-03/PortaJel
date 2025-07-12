using System.Collections.ObjectModel;
using Portajel.Connections.Database;
using Portajel.Desktop.Structures.ViewModel.Components;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel.Music;

public class ArtistViewModel : ReactiveObject, IRoutableViewModel
{
    public RoutingState Router { get; } = new RoutingState();

    public string? UrlPathSegment { get; }
    public IScreen HostScreen { get; }
    public ArtistData Artist { get; set; } 
    public ObservableCollection<AlbumData> Similar { get; set; } = [];
    public string AlbumSubtitle => $"{Artist.Name}  •  {SongCount} Songs, {Time}";
    private int SongCount = 0;
    private string Time = "56 Minutes";
    public string ImgSource
    {
        get => _imgSource;
        set => this.RaiseAndSetIfChanged(ref _imgSource, value);
    }
    private string _imgSource;
    public ObservableCollection<SongData> Songs { get; set; } = new();
    public HorizontalMusicViewModel HorizontalMusicViewModel { get; set; } = new();
    public ArtistViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public ArtistViewModel(IScreen screen, ArtistData artist)
    {
        HostScreen = screen;
        Artist = artist;
        ImgSource = Artist.ImgSource;
        HorizontalMusicViewModel.Title = "More like this";
    }
}