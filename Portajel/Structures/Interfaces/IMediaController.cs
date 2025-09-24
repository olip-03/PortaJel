using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Interfaces
{
    public interface IMediaController
    {
        IPlaybackController Playback { get; }
        IQueueController Queue { get; }
        void Initialize();
        void Update();
        void Destroy();
    }

    public interface IMediaEventSource
    {
        event EventHandler<InitializedEventArgs> Initialized;
    }
    
    public class InitializedEventArgs(
        IPlaybackController playback,
        IQueueController queueController)
        : EventArgs
    {
        public IPlaybackController playback { get; } = playback;
        public IQueueController QueueController { get; } = queueController;
    }
}
