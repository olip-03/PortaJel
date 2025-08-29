using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core.Extensions;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;

namespace Portajel.Structures.ViewModels.Pages.Views;

public class ArtistPageViewModel: ArtistData, INotifyPropertyChanged
{

    public double BackgroundHeight { get; set; } = 0;
    
    private ObservableCollection<AlbumData>? _albums;
    public ObservableCollection<AlbumData>? Albums 
    { 
        get => _albums;
        set
        {
            if (_albums != value)
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }
    }    
    public event PropertyChangedEventHandler? PropertyChanged;

    public ArtistPageViewModel()
    {

    }
    
    public void Update(BaseData[] albums, ArtistData? data)
    {
        if (data != null)
        {
            Id                         = data.Id;
            ServerId                   = data.ServerId;
            MediaType                  = data.MediaType;
            Name                       = data.Name;
            IsFavourite                = data.IsFavourite;
            PlayCount                  = data.PlayCount;
            DateAdded                  = data.DateAdded;
            DatePlayed                 = data.DatePlayed;
            ServerAddress              = data.ServerAddress;
            ImgSource                  = data.ImgSource;
            ImgBlurhash                = data.ImgBlurhash;
            ImgBlurhashSource          = data.ImgBlurhashSource;
            Description                = data.Description;
            LogoImgSource              = data.LogoImgSource;
            BackgroundImgSource        = data.BackgroundImgSource;
            BackgroundImgBlurhashSource = data.BackgroundImgBlurhashSource;
        }

        Albums = albums
            .Select(a => a.ToAlbum())
            .OrderBy(a => a.DateAdded)
            .ThenBy(a => a.Name)
            .ToObservableCollection();
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}