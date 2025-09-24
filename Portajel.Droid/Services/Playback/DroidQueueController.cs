using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;
using Portajel.Structures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Media3.Common;
using AndroidX.Media3.ExoPlayer;
using PortaJel.Droid.Services;

namespace Portajel.Droid.Playback
{
    public class DroidQueueController(DroidServiceController serviceController) : IQueueController
    {
        public List<SongData> PreviousQueue { get; set; } = new();
        public List<SongData> UpNextList { get; set; } = new();
        public KeyValuePair<BaseData, SongData[]>? CurrentCollection { get; set; } = null;
        public event EventHandler<QueueChangedEventArgs>? QueueChanged;
        public new SongData CurrentSong => UpNextList[0];
        public void AddSong(SongData toAdd, int? index = null)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.AddSong(toAdd, index);
            QueueChangedEventArgs args = new(QueueChangeKind.Add, [toAdd]);
            QueueChanged?.Invoke(this, args);
        }

        public void AddSong(SongData[] toAdd, int? index = null)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.AddSong(toAdd, index);
            QueueChangedEventArgs args = new(QueueChangeKind.Add, toAdd);
            QueueChanged?.Invoke(this, args);
        }

        public void ClearCollection(bool removeFromQueue)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.ClearCollection(removeFromQueue);
        }

        public void Previous()
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.Previous();
        }

        public void RemoveRange(int fromIndex, int toIndex)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.RemoveRange(fromIndex, toIndex);
        }

        public void RemoveSong(int index)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.RemoveSong(index);
        }

        public void Skip()
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.Skip();
        }

        public void SetCollection(BaseData collection, SongData[] collectionData, int fromIndex)
        {
            if (serviceController.AppServiceConnection.Binder == null)
                throw GetNullReferenceException();
            serviceController.AppServiceConnection.Binder.MediaController.Queue.SetCollection(collection, collectionData, fromIndex);
        }
        
        private NullReferenceException GetNullReferenceException()
        {
            return new NullReferenceException("Service not initalized! Check back later.");
        }
    }
}
