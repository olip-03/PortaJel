using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Portajel.Connections.Structs;
using Portajel.Structures.Interfaces;
using Portajel.Structures.ViewModels.Components;
using System;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Portajel.Components.Media
{
    public partial class MiniPlayer : ContentView
    {
        private readonly Timer _timer;
        private TimeSpan _elapsed;

        private readonly MediaPlayerViewModel _viewModel = new();
        private IMediaController _mediaController;

        public MiniPlayer(IMediaController mediaController)
        {
            _mediaController = mediaController;

            InitializeComponent();
            BindingContext = _viewModel;

            _elapsed = TimeSpan.Zero;
            _timer = new Timer(20); // 200ms interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
            _ = InitializeEventsAsync();
        }

        private async Task InitializeEventsAsync()
        {
            bool success = false;
            while (!success)
            {
                try
                {
                    if (_mediaController.Queue is IQueueEventSource events)
                    {
                        Trace.WriteLine("Miniplayer successfully registered events!!");
                        events.QueueChanged += Events_QueueChanged;
                        success = true; 
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Error accessing DroidQueue: {ex.Message}");
                }
                if (!success)
                {
                    await Task.Delay(500);
                }
            }
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
                await window.Navigation.PushModalAsync(new ModalPlayer()
                {
                    BindingContext = _viewModel
                });
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
    }
}