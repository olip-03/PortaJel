using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Jellyfin.Feeds;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Jellyfin
{
    public class JellyfinConnectorFeeds : ConnectorFeeds
    {
        private readonly IDbConnector? _database;
        private readonly string _serverUrl = "";
        public JellyfinConnectorFeeds(IDbConnector database, string serverUrl, List<BaseMediaFeed>? feeds = null)
        {
            _database = database;
            _serverUrl = serverUrl;
            InitializeFeeds();
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
                false,
                FeedViewStyle.HorizontalGrid
            );
            
            Add(recentlyListened.Id, recentlyListened); 
            Add(recentlyAdded.Id, recentlyAdded);
            Add(mostPlayed.Id, mostPlayed);
        }
    }
}