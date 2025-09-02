
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Interfaces;
using Portajel.Structures.ViewModels.Pages.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Portajel.Pages.Views
{
    public partial class AlbumPage : ContentPage, IQueryAttributable
    {
        private readonly IDbConnector _database;
        private readonly IServerConnector _server;
        private readonly IMediaController _mediaController;


        private AlbumPageViewModel _viewModel = new();

        private bool _isPlaying;
        public AlbumPage(IDbConnector database, IServerConnector server, IMediaController mediaController)
    	{
            _database = database;
            _server = server;
            _mediaController = mediaController;
            InitializeComponent();
            // GetNewAlbumData();
            Header.UpdateBackgroundOpacity(0);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var objects = query;
            foreach (var item in objects)
            {
                try
                {
                    if (item.Value is AlbumData itemValue)
                    {
                        _viewModel = new([], itemValue);
                    }
                    BindingContext = _viewModel;
                }
                catch
                {
                    return;
                }
            }
            Animate();
        }
        
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            Update();
        }
        
        private async void Update()
        {
            try
            {
                var localSongs = _database.Connectors.Song.GetAll(parentId: _viewModel.ServerId).ToArray();
                _viewModel.Update(localSongs, null);
                Animate();
                
                var server = _server.Servers[_viewModel.ServerAddress];
                if (server != null)
                {
                    var id = _viewModel.ServerId;

                    var albumTask = server.DataConnectors["Album"].GetAsync(id);
                    var songTask = server.DataConnectors["Song"].GetAllAsync(parentId: id);

                    await Task.WhenAll(albumTask, songTask);
                
                    var album = albumTask.Result.ToAlbum();
                    var songs = songTask.Result;
                    
                    _viewModel.Update(songs, album);
                }
            }
            catch (HttpRequestException dle)
            {
                Trace.WriteLine(dle.Message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                if (SongContainer.Opacity < 1)
                {
                    Animate();
                }
            }
        }

        private async Task<AlbumPageViewModel?> Download()
        {

            return null;
        }

        private async void Animate()
        {
            try
            {
                await SongContainer.FadeTo(1, 500, Easing.SinInOut);
            }
            catch
            {
                SongContainer.Opacity = 1;
            }
        }
        
        private async void CircleButton_OnClicked(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("..", true);
            }
            catch
            {
                // ignored
            }
        }

        private void PlayPause_OnClicked(object? sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            _viewModel.PlayPauseIcon = _isPlaying ? "media_pause.png" : "media_play.png";
        }
        
        private void RefreshView_OnRefreshing(object? sender, EventArgs e)
        {
            if (sender is RefreshView refreshView)
            {
                Update();
                refreshView.IsRefreshing = false;
            }
        }
        
        private async void OnLikeSwipeItemInvoked(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implementation
                if (sender is not SwipeItem swipeItem) return;
                if (swipeItem.BindingContext is not SongData song) return;
                var toast = Toast.Make($"{song.Name} added to favourites");
                await toast.Show();
            }
            catch
            {
                Trace.WriteLine("AlbumPage: Failed to add song to favourites");
            }
        }

        private async void OnAddToQueueSwipeItemInvoked(object sender, EventArgs e)
        {
            if (sender is not SwipeItem swipeItem) return;
            if (swipeItem.BindingContext is not SongData song) return;
            try
            {
                _mediaController.Queue.AddSong(song);

                var toast = Toast.Make($"{song.Name} added to queue");
                await toast.Show();
            }
            catch
            {
                Trace.WriteLine("AlbumPage: Failed to add song to queue");
            }
        }

        private void ScrollView_OnScrolled(object sender, ScrolledEventArgs e)
        {
            const double minScroll = 100.0;
            const double maxScroll = 300.0;
            double scroll = e.ScrollY - minScroll;
            if (scroll > 0)
            {
                double opacity = Math.Clamp(scroll / (maxScroll - minScroll), 0.0, 1.0);
                Header.UpdateBackgroundOpacity(opacity);
                if (scroll >= 1)
                {
                    
                }
            }
            else
            {
                Header.UpdateBackgroundOpacity(0);
            }
        }

        private async void Header_OnBackButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("..", true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}