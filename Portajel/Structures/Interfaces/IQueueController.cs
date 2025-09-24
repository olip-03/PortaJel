using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Structures.Interfaces
{
    public interface IQueueController
    {
        public event EventHandler<QueueChangedEventArgs>? QueueChanged;
        // Get Current Song
        public SongData CurrentSong { get; }

        // Get Current Collection
        public KeyValuePair<BaseData, SongData[]>? CurrentCollection { get; set; }

        /// <summary>
        /// Function to skip the song, to the next
        /// </summary>
        public abstract void Skip();

        // Function to skip back to the prior song
        public abstract void Previous();

        // Function that allows you to add a song to the queue
        public abstract void AddSong(SongData toAdd, int? index = null);

        // Function that allows you to add several songs to the queue
        public abstract void AddSong(SongData[] toAdd, int? index = null);

        // Function to remove songs
        public abstract void RemoveSong(int index);

        // Function to remove several songs
        public abstract void RemoveRange(int fromIndex, int toIndex);

        // Function to set the playing collection. Accepts BaseData as a playlist or 
        // an Album
        public abstract void SetCollection(BaseData collection, SongData[] collectionData, int fromIndex);

        // Removes the current playing collection. If removeFromQueue is true
        // songs from collection are removed. If false, tracks up next in the playing 
        // collection are first up in queue.
        public abstract void ClearCollection(bool removeFromQueue);
    }
}
