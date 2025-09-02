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

namespace Portajel.Droid.Playback
{
    public class DroidQueueController(IExoPlayer player) : IQueueController, IQueueEventSource
    {
        public event EventHandler<QueueChangedEventArgs>? QueueChanged;

        public List<SongData> PreviousQueue { get; set; } = new();
        public List<SongData> UpNextList { get; set; } = new();

        public KeyValuePair<BaseData, SongData[]>? CurrentCollection { get; set; } = null;
        public SongData CurrentSong => UpNextList[0];

        public void AddSong(SongData toAdd, int? index = null)
        {
            if (index == null)
            {
                UpNextList.Add(toAdd);
            }
            else
            {
                UpNextList.Insert(index.Value, toAdd);
            }

            var changedArgs = new QueueChangedEventArgs(
                QueueChangeKind.Add,
                new[] { toAdd },
                index ?? -1
            );
            QueueChanged?.Invoke(this, changedArgs);
        }

        public void AddSong(SongData[] toAdd, int? index = null)
        {
            if (index == null)
            {
                UpNextList.AddRange(toAdd);
            }
            else
            {
                UpNextList.InsertRange(index.Value, toAdd);
            }

            var changedArgs = new QueueChangedEventArgs(
                QueueChangeKind.Add,
                toAdd,
                index ?? -1
            );
            QueueChanged?.Invoke(this, changedArgs);
        }

        public void ClearCollection(bool removeFromQueue)
        {

        }

        public void Previous()
        {
            player.SeekToPrevious();
        }

        public void RemoveRange(int fromIndex, int toIndex)
        {

        }

        public void RemoveSong(int index)
        {

        }

        public void SetCollection(AlbumData collection, int fromIndex)
        {

        }

        public void Skip()
        {
            player.SeekToNext();
        }

        public void SetCollection(BaseData collection, SongData[] collectionData, int fromIndex)
        {
            CurrentCollection = new KeyValuePair<BaseData, SongData[]>(collection, collectionData);
            player.SeekTo(fromIndex, 0);
            // Todo: Tell the player where to start, and get it running from that point
        }
    }
}
