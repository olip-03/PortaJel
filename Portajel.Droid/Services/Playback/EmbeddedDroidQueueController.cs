using AndroidX.Media3.ExoPlayer;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;
using Portajel.Structures.Interfaces;

namespace Portajel.Droid.Playback;

public class EmbeddedDroidQueueController(IExoPlayer player) : IQueueController
{
    public List<SongData> PreviousQueue { get; set; } = new();
    public List<SongData> UpNextList { get; set; } = new();
    public KeyValuePair<BaseData, SongData[]>? CurrentCollection { get; set; } = null;
    public event EventHandler<QueueChangedEventArgs>? QueueChanged;
    public new SongData CurrentSong => UpNextList[0];

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