namespace Portajel.Connections.Services.Jellyfin.Dto;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class JfAuthRootInfo
{
    public JfUser User { get; set; }
    public JfSessionInfo SessionInfo { get; set; }
    public string AccessToken { get; set; }
    public string ServerId { get; set; }
}

public class JfUser
{
    public string Name { get; set; }
    public string ServerId { get; set; }
    public string Id { get; set; }
    public bool HasPassword { get; set; }
    public bool HasConfiguredPassword { get; set; }
    public bool HasConfiguredEasyPassword { get; set; }
    public bool EnableAutoLogin { get; set; }
    public DateTime LastLoginDate { get; set; }
    public DateTime LastActivityDate { get; set; }
    public JfConfiguration Configuration { get; set; }
    public JfPolicy Policy { get; set; }
}

public class JfConfiguration
{
    public bool PlayDefaultAudioTrack { get; set; }
    public string SubtitleLanguagePreference { get; set; }
    public bool DisplayMissingEpisodes { get; set; }
    public List<object> GroupedFolders { get; set; }
    public string SubtitleMode { get; set; }
    public bool DisplayCollectionsView { get; set; }
    public bool EnableLocalPassword { get; set; }
    public List<object> OrderedViews { get; set; }
    public List<object> LatestItemsExcludes { get; set; }
    public List<object> MyMediaExcludes { get; set; }
    public bool HidePlayedInLatest { get; set; }
    public bool RememberAudioSelections { get; set; }
    public bool RememberSubtitleSelections { get; set; }
    public bool EnableNextEpisodeAutoPlay { get; set; }
    public string CastReceiverId { get; set; }
}

public class JfPolicy
{
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool EnableCollectionManagement { get; set; }
    public bool EnableSubtitleManagement { get; set; }
    public bool EnableLyricManagement { get; set; }
    public bool IsDisabled { get; set; }
    public List<object> BlockedTags { get; set; }
    public List<object> AllowedTags { get; set; }
    public bool EnableUserPreferenceAccess { get; set; }
    public List<object> AccessSchedules { get; set; }
    public List<object> BlockUnratedItems { get; set; }
    public bool EnableRemoteControlOfOtherUsers { get; set; }
    public bool EnableSharedDeviceControl { get; set; }
    public bool EnableRemoteAccess { get; set; }
    public bool EnableLiveTvManagement { get; set; }
    public bool EnableLiveTvAccess { get; set; }
    public bool EnableMediaPlayback { get; set; }
    public bool EnableAudioPlaybackTranscoding { get; set; }
    public bool EnableVideoPlaybackTranscoding { get; set; }
    public bool EnablePlaybackRemuxing { get; set; }
    public bool ForceRemoteSourceTranscoding { get; set; }
    public bool EnableContentDeletion { get; set; }
    public List<object> EnableContentDeletionFromFolders { get; set; }
    public bool EnableContentDownloading { get; set; }
    public bool EnableSyncTranscoding { get; set; }
    public bool EnableMediaConversion { get; set; }
    public List<object> EnabledDevices { get; set; }
    public bool EnableAllDevices { get; set; }
    public List<object> EnabledChannels { get; set; }
    public bool EnableAllChannels { get; set; }
    public List<string> EnabledFolders { get; set; }
    public bool EnableAllFolders { get; set; }
    public int InvalidLoginAttemptCount { get; set; }
    public int LoginAttemptsBeforeLockout { get; set; }
    public int MaxActiveSessions { get; set; }
    public bool EnablePublicSharing { get; set; }
    public List<object> BlockedMediaFolders { get; set; }
    public List<object> BlockedChannels { get; set; }
    public int RemoteClientBitrateLimit { get; set; }
    public string AuthenticationProviderId { get; set; }
    public string PasswordResetProviderId { get; set; }
    public string SyncPlayAccess { get; set; }
}

public class JfSessionInfo
{
    public JfPlayState PlayState { get; set; }
    public List<object> AdditionalUsers { get; set; }
    public JfCapabilities Capabilities { get; set; }
    public string RemoteEndPoint { get; set; }
    public List<object> PlayableMediaTypes { get; set; }
    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Client { get; set; }
    public DateTime LastActivityDate { get; set; }
    public DateTime LastPlaybackCheckIn { get; set; }
    public string DeviceName { get; set; }
    public string DeviceId { get; set; }
    public string ApplicationVersion { get; set; }
    public bool IsActive { get; set; }
    public bool SupportsMediaControl { get; set; }
    public bool SupportsRemoteControl { get; set; }
    public List<object> NowPlayingQueue { get; set; }
    public List<object> NowPlayingQueueFullItems { get; set; }
    public bool HasCustomDeviceName { get; set; }
    public string ServerId { get; set; }
    public List<object> SupportedCommands { get; set; }
}

public class JfPlayState
{
    public bool CanSeek { get; set; }
    public bool IsPaused { get; set; }
    public bool IsMuted { get; set; }
    public string RepeatMode { get; set; }
    public string PlaybackOrder { get; set; }
}

public class JfCapabilities
{
    public List<object> PlayableMediaTypes { get; set; }
    public List<object> SupportedCommands { get; set; }
    public bool SupportsMediaControl { get; set; }
    public bool SupportsPersistentIdentifier { get; set; }
}