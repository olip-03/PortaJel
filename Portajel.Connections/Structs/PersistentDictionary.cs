using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portajel.Connections.Structs
{
    public class PersistentDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        private readonly SQLiteConnection _database;
        private const SQLiteOpenFlags DbFlags =
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

        public PersistentDictionary(string databasePath)
        {
            _database = new SQLiteConnection(databasePath, DbFlags);
            _database.EnableWriteAheadLogging();
            _database.CreateTable<DictionaryItemData>();
            LoadExistingItems();
        }

        private void LoadExistingItems()
        {
            var existingItems = _database.Table<DictionaryItemData>()
                .OrderBy(item => item.Id)
                .ToList();

            foreach (var item in existingItems)
            {
                try
                {
                    var deserializedKey = JsonSerializer.Deserialize<TKey>(item.SerializedKey);
                    var deserializedValue = JsonSerializer.Deserialize<TValue>(item.SerializedValue);

                    if (deserializedKey != null && deserializedValue != null)
                    {
                        base[deserializedKey] = deserializedValue;
                    }
                }
                catch
                {
                    _database.Delete(item);
                }
            }
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                try
                {
                    var serializedKey = JsonSerializer.Serialize(key);
                    var serializedValue = JsonSerializer.Serialize(value);

                    // Check if key already exists in database
                    var existingItem = _database.Table<DictionaryItemData>()
                        .FirstOrDefault(item => item.SerializedKey == serializedKey);

                    if (existingItem != null)
                    {
                        // Update existing item
                        existingItem.SerializedValue = serializedValue;
                        existingItem.UpdatedAt = DateTime.UtcNow;
                        _database.Update(existingItem);
                    }
                    else
                    {
                        // Insert new item
                        var dictionaryItem = new DictionaryItemData
                        {
                            SerializedKey = serializedKey,
                            SerializedValue = serializedValue,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _database.Insert(dictionaryItem);
                    }

                    base[key] = value;
                }
                catch (Exception ex)
                {
                    base[key] = value;
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        public new bool Remove(TKey key)
        {
            if (!ContainsKey(key))
                return false;

            try
            {
                var serializedKey = JsonSerializer.Serialize(key);
                var existingItem = _database.Table<DictionaryItemData>()
                    .FirstOrDefault(item => item.SerializedKey == serializedKey);

                if (existingItem != null)
                {
                    _database.Delete(existingItem);
                }
            }
            catch
            {
                // Continue even if database operation fails
            }

            return base.Remove(key);
        }

        public new void Clear()
        {
            base.Clear();

            try
            {
                _database.DeleteAll<DictionaryItemData>();
            }
            catch
            {

            }
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            if (ContainsKey(key))
                return false;

            this[key] = value;
            return true;
        }

        public new bool TryGetValue(TKey key, out TValue value)
        {
            return base.TryGetValue(key, out value);
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }

    [Table("DictionaryItems")]
    public class DictionaryItemData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("serialized_key")]
        public string SerializedKey { get; set; } = string.Empty;

        [Column("serialized_value")]
        public string SerializedValue { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}