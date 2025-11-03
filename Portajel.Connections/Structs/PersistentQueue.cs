using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portajel.Connections.Structs
{
    public class PersistentQueue<T> : Queue<T>
    {
        private readonly SQLiteConnection _database;
        private const SQLiteOpenFlags DbFlags =
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

        public PersistentQueue(string databasePath)
        {
            _database = new SQLiteConnection(databasePath, DbFlags);
            _database.EnableWriteAheadLogging();
            _database.CreateTable<QueueItemData>();
            LoadExistingItems();
        }

        private void LoadExistingItems()
        {
            var existingItems = _database.Table<QueueItemData>()
                .OrderBy(item => item.Id)
                .ToList();

            foreach (var item in existingItems)
            {
                try
                {
                    var deserializedItem = JsonSerializer.Deserialize<T>(item.SerializedData);
                    if (deserializedItem != null)
                    {
                        base.Enqueue(deserializedItem);
                    }
                }
                catch
                {
                    _database.Delete(item);
                }
            }
        }

        public new void Enqueue(T item)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(item);
                var queueItem = new QueueItemData
                {
                    SerializedData = serializedData,
                    CreatedAt = DateTime.UtcNow
                };

                _database.Insert(queueItem);
                base.Enqueue(item);
            }
            catch
            {
                base.Enqueue(item);
            }
        }

        public new T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var result = base.Dequeue();

            try
            {
                var oldestItem = _database.Table<QueueItemData>()
                    .OrderBy(item => item.Id)
                    .FirstOrDefault();

                if (oldestItem != null)
                {
                    _database.Delete(oldestItem);
                }
            }
            catch
            {

            }

            return result;
        }

        public new void Clear()
        {
            base.Clear();

            try
            {
                _database.DeleteAll<QueueItemData>();
            }
            catch
            {
                
            }
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }

    [Table("QueueItems")]
    public class QueueItemData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("serialized_data")]
        public string SerializedData { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
