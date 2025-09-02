using Portajel.Structures.Interfaces;
using PortaJel.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Services.Playback
{
    public class DroidMediaController : IMediaController
    {
        private Droid.Services.ServiceCollection _serviceConnection = null!;
        public DroidMediaController(DroidServiceController serverConnectior)
        {
            _serviceConnection = serverConnectior.AppServiceConnection;
        }

        public IPlaybackController Playback => _serviceConnection?.Binder?.MediaController.Playback ?? throw GetNullReferenceException();
        public IQueueController Queue => _serviceConnection?.Binder?.MediaController.Queue ?? throw GetNullReferenceException();

        public void Destroy()
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
        }

        public void Initialize()
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            _serviceConnection.Binder.MediaController.Initialize();
        }

        public void Update()
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            _serviceConnection.Binder.MediaController.Update();
        }

        private NullReferenceException GetNullReferenceException()
        {
            return new NullReferenceException("Service not initalized! Check back later.");
        }
    }
}
