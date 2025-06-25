using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Desktop.Structures.ViewModel.Components;
using ReactiveUI;
using SQLitePCL;

namespace Portajel.Desktop.Structures.ViewModel.Music;

public class AlbumViewModel : ReactiveObject, IRoutableViewModel
{
    public RoutingState Router { get; } = new RoutingState();

    public string? UrlPathSegment { get; }
    public IScreen HostScreen { get; }
    public AlbumData Album { get; set; } 
    public ObservableCollection<AlbumData> Similar { get; set; } = [];
    public string AlbumSubtitle => $"{Album.ArtistNames}  •  {SongCount} Songs, {Time}";
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
    public AlbumViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public AlbumViewModel(IScreen screen, AlbumData album)
    {
        HostScreen = screen;
        Album = album;
        ImgSource = Album.ImgSource;
        HorizontalMusicViewModel.Title = "More like this";
    }
}