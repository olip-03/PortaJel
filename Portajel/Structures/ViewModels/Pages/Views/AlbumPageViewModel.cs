using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Adapters;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Connections.Structs;

namespace Portajel.Structures.ViewModels.Pages.Views
{
    public class AlbumPageViewModel: AlbumData, INotifyPropertyChanged
    {
        private ObservableCollection<SongData>? _songs;
        public ObservableCollection<SongData>? Songs 
        { 
            get => _songs;
            set
            {
                if (_songs != value)
                {
                    _songs = value;
                    OnPropertyChanged(nameof(Songs));
                }
            }
        }        
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
        public event PropertyChangedEventHandler PropertyChanged;

        public AlbumPageViewModel() 
        {

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public AlbumPageViewModel(BaseData[] songs, AlbumData data)
        {
            Update(songs, data);
        }

        public void Update(BaseData[] songs, AlbumData? data)
        {
            if (data != null)
            {
                Id = data.Id;
                ServerId = data.ServerId;
                MediaType = data.MediaType;
                Name = data.Name;
                IsFavourite = data.IsFavourite;
                PlayCount = data.PlayCount;
                DateAdded = data.DateAdded;
                DatePlayed = data.DatePlayed;
                ServerAddress = data.ServerAddress;
                ImgSource = data.ImgSource;
                ImgBlurhash = data.ImgBlurhash;
                ImgBlurhashSource = data.ImgBlurhashSource;
                ArtistIdsJson = data.ArtistIdsJson;
                ArtistNames = data.ArtistNames;
            }
            Songs = songs.Select(s => s.ToSong()).
                OrderBy(s => s.DiskNumber).
                ThenBy(s => s.IndexNumber).
                ToObservableCollection();
        }
    }
}
