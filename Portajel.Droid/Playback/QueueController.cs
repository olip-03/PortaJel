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

namespace Portajel.Droid.Playback
{
    public class QueueController : IQueueController
    {
        public List<SongData> PreviousQueue { get; set; }
        public KeyValuePair<MediaType, BaseData> CurrentSong => throw new NotImplementedException();
        public KeyValuePair<MediaType, BaseData> CurrentColleciton => throw new NotImplementedException();
        public List<SongData> UpNextList { get; set; }

        public void AddSong(SongData toAdd, int index)
        {
            throw new NotImplementedException();
        }

        public void AddSong(SongData[] toAdd, int index)
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

        public void SetCollection(AlbumData collection, int fromIndex)
        {
            throw new NotImplementedException();
        }

        public void Skip()
        {
            throw new NotImplementedException();
        }
    }
}
