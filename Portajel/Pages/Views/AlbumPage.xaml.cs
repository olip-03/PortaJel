
using ExCSS;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Structures.ViewModels.Pages.Views;
using Portajel.Structures.ViewModels.Settings.Connections;
using System.Data;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;

namespace Portajel.Pages.Views
{
    public partial class AlbumPage : ContentPage, IQueryAttributable
    {
        private IDbConnector _database = default!;
        private IServerConnector _server = default!;

        private string url = default!;
        private Dictionary<string, AlbumData> _connectionProperties = new();
    
        private AlbumPageViewModel _viewModel = new();

        private bool _isPlaying = false;
        public AlbumPage(IDbConnector database, IServerConnector server)
    	{
            _database = database;
            _server = server;
            InitializeComponent();
            // GetNewAlbumData();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var objects = query;
            foreach (var item in objects)
            {
                try
                {
                    AlbumData? itemValue = item.Value as AlbumData;
                    _viewModel = new([], itemValue);
                    BindingContext = _viewModel;
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            AlbumData? connectorProperty = new AlbumData();
            var test = _connectionProperties.TryGetValue("URL", out connectorProperty);
        }
        
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            Update();
        }
        
        private async void Update(bool retry = true)
        {
            var localSongs = _database.Connectors.Song.GetAll(parentId: _viewModel.Id).Cast<SongData>();
            _viewModel.Songs = localSongs.ToObservableCollection();
            Animate();

            try
            {
                var server = _server.Servers[_viewModel.ServerAddress];
                if (server != null)
                {
                    var id = _viewModel.ServerId;

                    var albumTask = server.DataConnectors["Album"].GetAsync(id);
                    var songTask = server.DataConnectors["Song"].GetAllAsync(parentId: id);

                    await Task.WhenAll(albumTask, songTask);

                    var album = albumTask.Result;
                    var songs = songTask.Result;

                    _viewModel.Songs = songs.Cast<SongData>().ToObservableCollection();
                    // Todo: create insert or replace functions for database
                    // _database.Connectors.Album.Insert(album);
                    // _database.Connectors.Song.InsertRange(songs);
                }
            }
            catch (HttpRequestException httpEx)
            {
                try
                {
                    var server = _server.Servers[_viewModel.ServerAddress];
                    if (server != null)
                    {
                        await server.AuthenticateAsync();
                    }
                    if (retry)
                    {
                        Update(false);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            catch (Exception e)
            {
                // TODO: Message informing user of error
                Trace.WriteLine(e.Message);
            }
            finally
            {
                Animate();
            }
        }

        private async void Animate()
        {
            try
            {
                await SongContainer.FadeTo(1, 500, Easing.Linear);
            }
            catch (Exception e)
            {
                SongContainer.Opacity = 1;
            }
        }
        
        private async void CircleButton_OnClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private void PlayPause_OnClicked(object? sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying)
            {
                _viewModel.PlayPauseIcon = "media_pause.png";
            }
            else
            {
                _viewModel.PlayPauseIcon = "media_play.png";
            }
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
                if (sender is not SwipeItem swipeItem) return;
                if (swipeItem.BindingContext is not SongData song) return;
                var toast = Toast.Make($"{song.Name} added to favourites");
                await toast.Show();
            }
            catch (Exception exception)
            {
                Trace.WriteLine("AlbumPage: Failed to add song to favourites");
            }
        }

        private async void OnAddToQueueSwipeItemInvoked(object sender, EventArgs e)
        {
            try
            {
                if (sender is not SwipeItem swipeItem) return;
                if (swipeItem.BindingContext is not SongData song) return;
                var toast = Toast.Make($"{song.Name} added to queue");
                await toast.Show();
            }
            catch (Exception exception)
            {
                Trace.WriteLine("AlbumPage: Failed to add song to queue");
            }
        }

        private async void ScrollView_OnScrolled(object sender, ScrolledEventArgs e)
        {
            const double minScroll = 280.0;
            const double maxScroll = 460.0;
            double scroll = e.ScrollY - minScroll;
            if (scroll > 0)
            {
                double opacity = Math.Clamp(scroll / (maxScroll - minScroll), 0.0, 1.0);
                HeaderBackground.Opacity = opacity;
                if (scroll >= 1)
                {
                    
                }
            }
            else
            {
                HeaderBackground.Opacity = 0;
            }
        }
    }
}