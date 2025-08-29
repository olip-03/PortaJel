using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Portajel.Components;

namespace Portajel.Structures.ViewModels.Pages
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        readonly FeedGridView _feedGridView;

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing == value) return;
                _isRefreshing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public HomePageViewModel(FeedGridView feedGridView)
        {
            _feedGridView = feedGridView ?? 
                            throw new ArgumentNullException(nameof(feedGridView));

            RefreshCommand = new Command(OnRefresh);
            NavigateSettingsCommand = new Command(async () =>
            {
                // navigate to your settings page
                await Shell.Current.GoToAsync("settings");
            });
        }

        void OnRefresh()
        {
            _feedGridView.RefreshFeeds();
            // finish pull-to-refresh
            IsRefreshing = false;
        }
    }
}