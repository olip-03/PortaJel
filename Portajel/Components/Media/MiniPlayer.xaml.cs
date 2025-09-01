using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Portajel.Components.Media
{
    public partial class MiniPlayer : ContentView
    {
        private readonly Timer _timer;
        private TimeSpan _elapsed;

        public MiniPlayer()
        {
            InitializeComponent();

            _elapsed = TimeSpan.Zero;
            _timer = new Timer(20); // 200ms interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
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

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Navigation.PushModalAsync(new ModalPlayer());
        }

        //// Optionally, stop the timer when disposing
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    _timer?.Stop();
        //    _timer?.Dispose();
        //}
    }
}