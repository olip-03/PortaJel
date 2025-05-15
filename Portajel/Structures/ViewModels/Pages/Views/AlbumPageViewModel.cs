using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Adapters;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.ViewModels.Pages.Views
{
    public class AlbumPageViewModel: AlbumData
    {
        public AlbumPageViewModel() 
        {

        }

        public ObservableCollectionAdapter<SongData>? Songs { get; set; }

        public AlbumPageViewModel(IEnumerable<SongData> songs, AlbumData data)
        {
            Id = data.Id;
            ServerId = data.ServerId;
            MediaType = data.MediaType;
            Name = data.Name;
            Index = data.Index;
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
            SongIdsJson = data.SongIdsJson;
            GetSimilarJson = data.GetSimilarJson;
            Songs = new ObservableCollectionAdapter<SongData>(songs.ToObservableCollection());
        }
    }
}
