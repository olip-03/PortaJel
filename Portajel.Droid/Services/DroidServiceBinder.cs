using Android.OS;
using Android.Widget;
using Portajel.Connections.Services.Database;
using Portajel.Connections;
using Portajel.Structures.Interfaces;

namespace Portajel.Droid.Services
{
    public class DroidServiceBinder : Binder, IDisposable
    {
        public IMediaController MediaController { get; private set; } = null!;
        public IPlaybackController PlaybackController => MediaController.Playback;
        public IQueueController QueueController => MediaController.Queue;
        public DatabaseConnector Database { get; private set; } = null!;
        public ServerConnector Server { get; private set; } = null!;
        public DroidService Service { get; private set; } = null!;
        public DroidServiceBinder(DroidService service)
        {
            Service = service;
            Server = Service.serverConnector;
            Database = Service.database;
            MediaController = service.MediaController;
        }
        public void Destroy()
        {

        }
    }
}
