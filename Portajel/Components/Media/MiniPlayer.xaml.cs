using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Portajel.Connections.Structs;
using Portajel.Structures.Interfaces;
using Portajel.Structures.ViewModels.Components;
using System;
using System.Diagnostics;
using System.Timers;
using Portajel.Connections.Database;
using Timer = System.Timers.Timer;

namespace Portajel.Components.Media
{
    public partial class MiniPlayer : ContentView
    {
        private readonly ModalPlayer modalPlayer;
        
        private readonly Timer _timer;
        private TimeSpan _elapsed;

        private MediaPlayerViewModel _viewModel = new();
        private IQueueController _queueController;

        public MiniPlayer(IQueueController queueController, IEnumerable<SongData>? songs = null)
        {
            _queueController = queueController;

            if (songs != null)
            {
                foreach (var song in songs)
                {
                    _viewModel.Queue.Add(song);
                }
            }

            InitializeComponent();
            BindingContext = _viewModel;
            modalPlayer = new ModalPlayer(_viewModel);
            modalPlayer.OnClose += OnModalClose;
            
            _elapsed = TimeSpan.Zero;
            _timer = new Timer(20); // 200ms interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
            _queueController.QueueChanged += Events_QueueChanged;

            TranslationY += 64;
            Animate();
        }
        
        private async void Animate()
        {
            await Task.Delay(100);
            await this.TranslateTo(0, 0, 700, Easing.CubicOut);
        }

        private void OnModalClose(object? sender, EventArgs e)
        {
            _viewModel.QueuePosition = modalPlayer.ViewModel.QueuePosition;
            SongCarousel.Position = _viewModel.QueuePosition;
        }

        private void Events_QueueChanged(object? sender, QueueChangedEventArgs e)
        {
            if(e.Kind == QueueChangeKind.Add)
            {
                foreach (var song in e.Songs)
                {
                    _viewModel.Queue.Add(song);
                }
            }
            // Queue has changed update state 
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Marshal to main thread for UI updates
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _elapsed = _elapsed.Add(TimeSpan.FromMilliseconds(200));
                if(TimeTracker.Progress >= 1)
                {
                    TimeTracker.Progress = 0;
                }
                // Example: Update a label or property
                // MyLabel.Text = _elapsed.ToString(@"mm\:ss\.ff");
                TimeTracker.Progress += 0.001;
            });
        }

        private async void OpenPlayer()
        {
            try
            {
                var app = Application.Current;
                var window = app?.Windows[0];
                if (window == null)
                    return;
                await window.Navigation.PushModalAsync(modalPlayer);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Failed to open player {e.Message}");
            }
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            OpenPlayer();
        }

        private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        {
            OpenPlayer();
        }
        
        private void CarouselView_OnCurrentItemChanged(object? sender, CurrentItemChangedEventArgs e)
        {
            if (sender is CarouselView carouselView)
            {
                _viewModel.QueuePosition = carouselView.Position;
            }
            if (e.CurrentItem is SongData current)
            {
                _viewModel.Current.Name = current.Name;
                _viewModel.Current.ArtistNames = current.ArtistNames;
            }
        }
    }
}