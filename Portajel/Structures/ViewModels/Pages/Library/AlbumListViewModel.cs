using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Portajel.Connections.Interfaces;
using Portajel.Structures.Adaptor;

namespace Portajel.Structures.ViewModels.Pages.Library
{
    public partial class DatabaseBindViewModel : ObservableObject
    {
        private IDbItemConnector _database;
        public DatabaseBindViewModel(IDbItemConnector database)
        {
            _database = database;
            Adapter = new MusicItemAdaptor(_database);
        }

        public MusicItemAdaptor? Adapter { get; set; }

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
