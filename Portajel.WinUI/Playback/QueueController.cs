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

namespace Portajel.WinUI.Playback
{
    public class QueueController : IQueueController
    {
        public SongData CurrentSong => throw new NotImplementedException();

        public KeyValuePair<BaseData, SongData[]>? CurrentCollection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<QueueChangedEventArgs>? QueueChanged;

        public void AddSong(SongData toAdd, int? index = null)
        {
            throw new NotImplementedException();
        }

        public void AddSong(SongData[] toAdd, int? index = null)
        {
            throw new NotImplementedException();
        }

        public void ClearCollection(bool removeFromQueue)
        {
            throw new NotImplementedException();
        }

        public void Previous()
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(int fromIndex, int toIndex)
        {
            throw new NotImplementedException();
        }

        public void RemoveSong(int index)
        {
            throw new NotImplementedException();
        }

        public void SetCollection(BaseData collection, SongData[] collectionData, int fromIndex)
        {
            throw new NotImplementedException();
        }

        public void Skip()
        {
            throw new NotImplementedException();
        }
    }
}
