
using ExCSS;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Structures.ViewModels.Pages.Views;
using Portajel.Structures.ViewModels.Settings.Connections;
using System.Data;

namespace Portajel.Pages.Views
{
    public partial class AlbumPage : ContentPage, IQueryAttributable
    {
        private IDbConnector _database = default!;
        private IServerConnector _server = default!;

        private string url = default!;
        private Dictionary<string, AlbumData> _connectionProperties = new();
    
        private AlbumPageViewModel _viewModel = new();

        public AlbumPage(IDbConnector database, IServerConnector server)
    	{
            _database = database;
            _server = server;
            InitializeComponent();
            GetNewAlbumData();
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

        private async void GetNewAlbumData()
        {
            if (_viewModel.Id == null) return;
            var servers = _server.Servers;

            var tasks = servers.Select(async server =>
            {
                var item = await server.GetDataConnectors()["Album"].GetAsync(_viewModel.Id.Value, _viewModel.ServerAddress);
                if(item is AlbumData album)
                {
                    var songIds = album.GetSongIds().Select(id => (Guid?)id).ToArray();
                    var songData = await server.GetDataConnectors()["Song"].GetAllAsync(includeIds: songIds);
                    var songs = songData.Select(s => (SongData)s);
                    _viewModel = new(songs, album);
                    BindingContext = _viewModel;
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}