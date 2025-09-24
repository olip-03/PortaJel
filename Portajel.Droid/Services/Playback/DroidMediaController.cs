using Portajel.Structures.Interfaces;
using PortaJel.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Services.Playback
{
    public class DroidMediaController(DroidServiceController serviceController) : IMediaController, IMediaEventSource
    {
        private readonly Droid.Services.ServiceConnection _serviceController = serviceController.AppServiceConnection;
        public event EventHandler<InitializedEventArgs>? Initialized;
        public IPlaybackController Playback => _serviceController?.Binder?.MediaController.Playback ?? throw GetNullReferenceException();
        public IQueueController Queue => _serviceController?.Binder?.MediaController.Queue ?? throw GetNullReferenceException();
        public void Destroy()
        {
            if (_serviceController.Binder == null)
                throw GetNullReferenceException();
        }
        public void Initialize()
        {
            if (_serviceController.Binder == null)
                throw GetNullReferenceException();
            _serviceController.Binder.MediaController.Initialize();
            Initialized?.Invoke(this, new(Playback, Queue));
        }
        public void Update()
        {
            if (_serviceController.Binder == null)
                throw GetNullReferenceException();
            _serviceController.Binder.MediaController.Update();
        }
        private NullReferenceException GetNullReferenceException()
        {
            return new NullReferenceException("Service not initalized! Check back later.");
        }
    }
}
