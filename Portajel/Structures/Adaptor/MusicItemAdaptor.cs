using Jellyfin.Sdk.Generated.Models;
using Microsoft.Maui.Adapters;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using SQLite;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Structures.Adaptor
{ 
    public class MusicItemAdaptor(IDbItemConnector database, bool favs, bool descendingOrder) : VirtualListViewAdapterBase<object, BaseData>
    {
        private int? totalCount = null;
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            SortOrder sortOrder = descendingOrder ? SortOrder.Descending : SortOrder.Ascending;
            switch (database)
            {
                case TableQuery<AlbumData> albumTable:
                    var sql = @"SELECT Id, Name, ArtistNames, ImgBlurhash, ImgSource FROM AlbumData 
                        WHERE (@favs IS NULL OR IsFavourite = @favs)
                        ORDER BY Name " + (sortOrder == SortOrder.Ascending ? "ASC" : "DESC") + @"
                        LIMIT 1 OFFSET @itemIndex";
                    return albumTable.Connection.Query<AlbumData>(sql, favs, itemIndex).FirstOrDefault();
                case TableQuery<ArtistData> artistTable:
                    
                    break;
                case TableQuery<PlaylistData> playlistData:
                    
                    break;
                case TableQuery<SongData> songData:
                    var songSql = @"
                        SELECT t.* 
                        FROM (
                            SELECT Id
                            FROM SongData
                            WHERE (@favs IS NULL OR IsFavourite = @favs)
                            ORDER BY Name " + (sortOrder == SortOrder.Ascending ? "ASC" : "DESC") + @"
                            LIMIT 1 OFFSET @itemIndex
                        ) q
                        JOIN SongData t ON t.Id = q.Id";
                    return songData.Connection.Query<SongData>(songSql, favs, itemIndex).FirstOrDefault();
                case TableQuery<GenreData> genreData:
                    
                    break;
            }
            var result = database.GetAll(
                limit: 1, 
                getFavourite: favs,
                startIndex: itemIndex, 
                setSortOrder: sortOrder, 
                setSortTypes: ItemSortBy.Name).First();
            return result;
        }
        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            totalCount ??= database.GetTotalCount(favs);
            return totalCount.Value;
        }
    }

    public class MemoryItemAdaptor() : VirtualListViewAdapterBase<object, BaseData>
    {
        public List<BaseData> Items = new();
        
        public override BaseData GetItem(int sectionIndex, int itemIndex)
        {
            return Items[itemIndex];
        }

        public override int GetNumberOfItemsInSection(int sectionIndex)
        {
            return Items.Count;
        }
        public void SetItems(IEnumerable<BaseData> items)
        {
            Items = items.ToList();
        }
    }
}