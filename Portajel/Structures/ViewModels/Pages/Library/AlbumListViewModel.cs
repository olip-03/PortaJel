using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;

namespace Portajel.Structures.ViewModels.Pages.Library
{
    public partial class DatabaseBindViewModel : ObservableObject
    {
        public EventHandler OnDataRefresh;    
        private IDbItemConnector _database;
        
        public DatabaseBindViewModel(IDbItemConnector database)
        {
            _database = database;
            Adapter = new MusicItemAdaptor(_database, false, false);
        }
        
        public MusicItemAdaptor? Adapter { get; set; }

        [ObservableProperty]
        private bool _filterFavourites = false;
        
        [ObservableProperty]
        private bool _orderDescending = false;

        [ObservableProperty]
        private string _favouriteIcon = "favourite_empty.png";
        
        [ObservableProperty]
        private string _orderIcon = "arrow_upward.png";
        
        partial void OnFilterFavouritesChanged(bool value)
        {
            FavouriteIcon = value ? "favourite_filled.png" : "favourite_empty.png";
            ResetAdapter();
        }

        partial void OnOrderDescendingChanged(bool value)
        {
            OrderIcon = value ? "arrow_downward.png" : "arrow_upward.png";
            ResetAdapter();
        }
        
        void ResetAdapter()
        {
            Adapter = new MusicItemAdaptor(_database, FilterFavourites, OrderDescending);
            OnDataRefresh.Invoke(null, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task Refresh(Action? completion)
        {
            ResetAdapter();
            await Task.Delay(300);
            completion?.Invoke();
        }

        [RelayCommand]
        private void Scrolled(ScrolledEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Scrolled: {e.ScrollX}, {e.ScrollY}");
        }
    }

}
