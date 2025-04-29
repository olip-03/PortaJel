using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;

namespace Portajel.Structures.ViewModels.Pages.Library
{
    public partial class AlbumListViewModel : ObservableObject
    {
        private IDbConnector _database;
        public AlbumListViewModel(IDbConnector database)
        {
            _database = database;
            Adapter = new MusicItemAdaptor(_database);
            Init();
        }

        private async void Init()
        {
            var total = _database.GetDataConnectors()["Album"].GetTotalCount();
            Adapter = new MusicItemAdaptor(_database, total);
        }

        [ObservableProperty]
        MusicItemAdaptor? adapter;

        [RelayCommand]
        async Task Refresh(Action completion)
        {
            Adapter = new MusicItemAdaptor(_database);
            System.Diagnostics.Debug.WriteLine("Refresh Complete");
            completion?.Invoke();
        }

        [RelayCommand]
        void Scrolled(ScrolledEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Scrolled: {e.ScrollX}, {e.ScrollY}");
        }
    }

}
