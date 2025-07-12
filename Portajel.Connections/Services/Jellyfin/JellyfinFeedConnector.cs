using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Jellyfin.Feeds;

namespace Portajel.Connections.Services.Jellyfin
{
    public class JellyfinFeedConnector : IFeedConnector
    {
        public Dictionary<string, IMediaFeed> AvailableFeeds { get; private set; } = new();
        private readonly IDbConnector? _database;
        private readonly string _serverUrl = "";
        public JellyfinFeedConnector(IDbConnector database, string serverUrl)
        {
            _database = database;
            _serverUrl = serverUrl;
            InitializeFeeds();
        }
        public void SetFeedState(string feedId, bool enabled = true)
        {
            if (AvailableFeeds.TryGetValue(feedId, out var feed))
            {
                feed.IsEnabled = enabled;
            }
        }

        private void InitializeFeeds()
        {
            if (_database == null || String.IsNullOrWhiteSpace(_serverUrl))
            {
                throw new InvalidOperationException(
                    "Feed connector must be initialized with Database and ServerURL before initializing feeds."
                );
            }
            
            var recentlyListened = new JellyfinRecentlyListened(
                _database, 
                _serverUrl, 
                false,
                FeedViewStyle.ListView
            );
            var recentlyAdded = new JellyfinRecentlyAdded(
                _database, 
                _serverUrl, 
                false,
                FeedViewStyle.HorizontalGrid
            );
            var mostPlayed = new JellyfinMostPlayed(
                _database, 
                _serverUrl, 
                false
            );
            AvailableFeeds = new Dictionary<string, IMediaFeed>
            {
                { recentlyListened.Id, recentlyListened },
                { recentlyAdded.Id, recentlyAdded },
                { mostPlayed.Id, mostPlayed }
            };
        }
    }
}