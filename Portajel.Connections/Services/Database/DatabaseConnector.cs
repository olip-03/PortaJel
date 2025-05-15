using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Database;
using SQLite;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Portajel.Connections.Data;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Services.Database
{
    public class DatabaseConnector : IDbConnector
    {
        public SQLiteConnection Database { get; }
        private const SQLiteOpenFlags DbFlags =
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
        public IDbItemConnector AlbumData { get; set; }
        public IDbItemConnector ArtistData { get; set; }
        public IDbItemConnector SongData { get; set; }
        public IDbItemConnector PlaylistData { get; set; }
        public IDbItemConnector Genre { get; set; }

        public DbConnectors Connectors { get; } 

        // TODO: Storing Radio Stations in Db
        // First we need to implement the API functions 

        public DatabaseConnector(string appDataDirectory)
        {
            Database = new SQLiteConnection(appDataDirectory, DbFlags);

            Database.EnableWriteAheadLogging();

            Database.CreateTable<AlbumData>();
            Database.CreateTable<SongData>();
            Database.CreateTable<ArtistData>();
            Database.CreateTable<PlaylistData>();
            Database.CreateTable<GenreData>();

            Connectors = new(
                new DatabaseAlbumConnector(Database),
                new DatabaseArtistConnector(Database),
                new DatabaseSongConnector(Database),
                new DatabasePlaylistConnector(Database),
                new DatabaseGenreConnector(Database)
            );
        }

        public BaseData[] Search(
            string searchTerm = "", 
            int limit = 50, 
            int startIndex = 0,
            ItemSortBy setSortTypes = ItemSortBy.Name, 
            SortOrder setSortOrder = SortOrder.Ascending,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If no search term is provided, return an empty array.
                return Array.Empty<BaseData>();
            }

            searchTerm = searchTerm.Trim();

            var resultList = new List<BaseData>();

            // Iterate over each data connector
            foreach (var connectorPair in Connectors.GetDataConnectors())
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var connectorName = connectorPair.Key;
                    var dataConnector = connectorPair.Value;

                    switch (connectorName)
                    {
                        case MediaCapabilities.Album:
                            var matchingAlbums = Database.Table<AlbumData>()
                                .Take(limit)
                                .Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()))
                                .ToList();
                            resultList.AddRange(matchingAlbums);
                            break;
                        case MediaCapabilities.Artist:
                            var matchingArtists = Database.Table<ArtistData>()
                                .Take(limit)
                                .Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()))
                                .ToList();
                            resultList.AddRange(matchingArtists);
                            break;
                        case MediaCapabilities.Song:
                            var matchingSongs = Database.Table<SongData>()
                                .Take(limit)
                                .Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()))
                                .ToList();
                            resultList.AddRange(matchingSongs);
                            break;
                        case MediaCapabilities.Playlist:
                            var matchingPlaylists = Database.Table<PlaylistData>()
                                .Take(limit)
                                .Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()))
                                .ToList();
                            resultList.AddRange(matchingPlaylists);
                            break;
                        case MediaCapabilities.Genre:
                            // var matchingItems = (await Database.Table<GenreData>()
                            //     .Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()))
                            //     .ToList()
                            //     )
                            //     .ToList();
                            // resultList.InsertRange(matchingItems.Select(item => new Genre(item)));
                            break;
                    }
                }
                catch 
                {
                    return Array.Empty<BaseData>();
                }
            }

            // Now, apply sorting on the combined results
            IEnumerable<BaseData> sortedResult;

            switch (setSortTypes)
            {
                case ItemSortBy.DateCreated:
                    sortedResult = setSortOrder == SortOrder.Ascending
                        ? resultList.OrderBy(item => item.DateAdded)
                        : resultList.OrderByDescending(item => item.DateAdded);
                    break;
                case ItemSortBy.DatePlayed:
                    sortedResult = setSortOrder == SortOrder.Ascending
                        ? resultList.OrderBy(item => item.DatePlayed)
                        : resultList.OrderByDescending(item => item.DatePlayed);
                    break;
                case ItemSortBy.Name:
                    sortedResult = setSortOrder == SortOrder.Ascending
                        ? resultList.OrderBy(item => item.Name)
                        : resultList.OrderByDescending(item => item.Name);
                    break;
                case ItemSortBy.PlayCount:
                    sortedResult = setSortOrder == SortOrder.Ascending
                        ? resultList.OrderBy(item => item.PlayCount)
                        : resultList.OrderByDescending(item => item.PlayCount);
                    break;
                default:
                    // Default sorting by Name
                    sortedResult = setSortOrder == SortOrder.Ascending
                        ? resultList.OrderBy(item => item.Name)
                        : resultList.OrderByDescending(item => item.Name);
                    break;
            }

            // Return the result as an array
            return sortedResult.ToArray();
        }
    }
}