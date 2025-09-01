using Portajel.Structures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Media.Session;
using AndroidX.Media3.Common;
using AndroidX.Media3.DataSource;
using AndroidX.Media3.ExoPlayer;
using AndroidX.Media3.ExoPlayer.Analytics;
using AndroidX.Media3.ExoPlayer.Source;
using AndroidX.Media3.Extractor;
using AndroidX.Media3.Extractor.Mp3;
using FFImageLoading;
using AudioAttributes = AndroidX.Media3.Common.AudioAttributes;

namespace Portajel.Droid.Playback
{
    public class DroidMediaController : IMediaController
    {
        public IPlaybackController DroidPlayback => _droidPlaybackController ?? throw new NullReferenceException("Initialize must be called before services can be used.");
        public IQueueController DroidQueue => _droidQueueController ?? throw new NullReferenceException("Initialize must be called before services can be used.");
        
        private const int NOTIFICATION_ID = 10000;
        private const string NOTIFICATION_CHANNEL_ID = "porta_jel_media";
        private MediaSession? _mediaSession;
        private Android.App.NotificationManager? _notificationManager;
        private IExoPlayer? _player;
        private IPlayerListener? _playerListener;

        private DroidPlaybackController? _droidPlaybackController = new();
        private DroidQueueController? _droidQueueController;
        
        public void Initialize()
        {
            var context = Platform.AppContext;
            CreatePlayer(context);
            
            _notificationManager ??= (NotificationManager)context.GetSystemService(Context.NotificationService)!;
            
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var channel = _notificationManager.GetNotificationChannel(NOTIFICATION_CHANNEL_ID);
                if (channel == null)
                {
                    channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "PortaJel playback", NotificationImportance.Max)
                    {
                        Description = "Playback controls for PortaJel"
                    };
                    channel.SetVibrationPattern([0]);
                    channel.EnableVibration(false);
                    _notificationManager.CreateNotificationChannel(channel);
                }
            }
        }

        private void CreatePlayer(Context context)
        {
            var httpFactory = new DefaultHttpDataSource.Factory()
                .SetAllowCrossProtocolRedirects(true);
            var extractorsFactory = new DefaultExtractorsFactory()?
                .SetConstantBitrateSeekingEnabled(true)?
                .SetMp3ExtractorFlags(Mp3Extractor.FlagEnableIndexSeeking);
            var mediaSourceFactory = new DefaultMediaSourceFactory(context, extractorsFactory)
                .SetDataSourceFactory(httpFactory);
            var audioAttributes = new AudioAttributes.Builder()?
                .SetUsage(1)? // 1 = media
                .SetContentType(2)? // 2 = music
                .Build();
            
            _player = new ExoPlayerBuilder(context)?.Build() ?? throw new Exception("Could not build ExoPlayer");
            
            _player.SetAudioAttributes(audioAttributes, true);
            _player.RepeatMode = 0;
            _player.Prepare(); // ok to prepare once; adding MediaItems later works
            _droidQueueController = new(_player);

            var listener = new PlaybackEventListener();
            _player.AddListener(listener); // your existing listener
        }

        private void CreateNotificationPlayer(Context context)
        {
            _mediaSession = new MediaSession(context, NOTIFICATION_CHANNEL_ID);
            PlaybackState.Builder psBuilder = new PlaybackState.Builder();
            
            _mediaSession.SetCallback();
        }
        
        public void Update()
        {
            
        }

        public void Destroy()
        {
            _player.Stop();
        }
    }
}
