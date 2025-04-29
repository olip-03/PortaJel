using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Connections.Data;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces
{
    public interface IPlaybackInterface
    {
        Task<bool> Initalize();
        void Destroy();
        void Play();
        void SetPlayAddonAction(Action addonAction);
        void Pause();
        void TogglePlay();
        void ToggleShuffle();
        void ToggleRepeat();
        void NextTrack();
        void PreviousTrack();
        void SeekToPosition(long position);
        void SeekToIndex(int index);
        void SetPlayingCollection(BaseData baseMusicItem, int fromIndex = 0);
        BaseData GetPlayingCollection();
        void AddSong(SongData song);
        void AddSongs(SongData[] songs);
        void RemoveSong(int index);
        // SongGroupCollection GetQueue();
        SongData GetCurrentlyPlaying();
        // public PlaybackInfo GetPlaybackTimeInfo();
        int GetQueueIndex();
        bool GetIsPlaying();
        bool GetIsShuffling();
        int GetRepeatMode();
    }
}
