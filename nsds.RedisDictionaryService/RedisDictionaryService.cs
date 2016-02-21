using nsds.Exceptions;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace nsds.RedisDictionaryService
{
    public class RedisDictionaryService : IDictionaryService, IDisposable
    {
        private string dictionaryServiceId = "RedisDictionaryService";
        private string connectionString;
        private ConnectionMultiplexer redis;
        private int databaseNumber;
        private int timeDataBaseNumber;

        public RedisDictionaryService(string connectionString, int databaseNumber = 0, int timeDatabaseNumber = 1,
            bool AllowAdmin = false)
        {
            if (databaseNumber == timeDatabaseNumber)
            {
                throw new NsdsException("Redis Database Number and TimeDatabaseNumber is the same database.", "-99");
            }
            this.connectionString = connectionString;
            this.databaseNumber = databaseNumber;
            this.timeDataBaseNumber = timeDatabaseNumber;
            var options = ConfigurationOptions.Parse(this.connectionString);
            options.AllowAdmin = AllowAdmin;
            this.redis = ConnectionMultiplexer.Connect(options);
        }

        public object this[string key]
        {
            get { return this.GetValue(key); }

            set { this.SetValue(key, value); }
        }

        public int Count
        {
            get { return this.GetRedisKeys(this.databaseNumber).Count; }
        }

        public string DictionaryServiceId
        {
            get { return this.dictionaryServiceId; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<string> Keys
        {
            get { return this.GetRedisKeys(this.databaseNumber); }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(string key, object value)
        {
            if (!value.GetType().IsSerializable)
            {
                throw new NotSerializableException("Value can not Serialize.");
            }
            this.SetValue(key, value);
        }

        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            if (!value.GetType().IsSerializable)
            {
                throw new NotSerializableException("Value can not Serialize.");
            }
            this.SetValue(key, value, slidingExpiration);
        }

        public object Get(string key)
        {
            return this.GetValue(key);
        }

        public void Clear()
        {
            var server = redis.GetServer(this.connectionString);
            server.FlushDatabase(this.databaseNumber);
            server.FlushDatabase(this.timeDataBaseNumber);
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            var db = this.GetRedisDb();
            return db.KeyExists(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new RedisDictionaryEnumerator(this);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            var tdb = this.GetRedisTimeDb();
            if (tdb.KeyExists(key))
            {
                tdb.KeyDelete(key);
            }
            return this.GetRedisDb().KeyDelete(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RedisDictionaryEnumerator(this);
        }

        public void Dispose()
        {
            this.redis.Close();
        }

        private IDatabase GetRedisDb()
        {
            return this.redis.GetDatabase(this.databaseNumber);
        }

        private IDatabase GetRedisTimeDb()
        {
            return this.redis.GetDatabase(this.timeDataBaseNumber);
        }

        private object GetValue(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new KeyNotFoundException(string.Format("{0} key not found.", key.ToString()));
            }
            var db = this.GetRedisDb();
            byte[] valueBa = db.StringGet(key);
            this.UpdateKeyExpire(key);
            return this.DeserializeObject(valueBa);
        }


        private void SetValue(string key, object value)
        {
            var db = this.GetRedisDb();
            db.StringSet(key, this.SerializeObject(value));
            this.UpdateKeyExpire(key);
        }

        private void SetExpValue(string key, TimeSpan expireTime)
        {
            var db = this.GetRedisTimeDb();
            db.StringSet(key, expireTime.TotalSeconds);
        }

        private void SetValue(string key, object value, TimeSpan expireTime)
        {
            this.SetValue(key, value);
            this.SetKeyExpire(key, expireTime);
            this.UpdateKeyExpire(key);
        }

        private void SetKeyExpire(string key, TimeSpan expireTime)
        {
            this.SetExpValue(key, expireTime);
        }

        private void UpdateKeyExpire(string key)
        {
            var tdb = this.GetRedisTimeDb();
            if (!tdb.KeyExists(key))
            {
                return;
            }
            var secs = (double) tdb.StringGet(key);
            var vdb = this.GetRedisDb();

            if (!vdb.KeyExists(key))
            {
                // Time Db has value but value db not. delete time db's key.
                tdb.KeyDelete(key);
                return;
            }

            vdb.KeyExpire(key, TimeSpan.FromSeconds(secs));
            tdb.KeyExpire(key, TimeSpan.FromSeconds(secs));
        }

        private List<string> GetRedisKeys(int dbNum)
        {
            var keylist = new List<string>();
            foreach (var key in this.redis.GetServer(this.connectionString).Keys(pattern: "*", database: dbNum))
            {
                keylist.Add((string) key);
            }
            return keylist;
        }

        private byte[] SerializeObject(object value)
        {
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        private object DeserializeObject(byte[] value)
        {
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(value))
            {
                return bf.Deserialize(ms);
            }
        }
    }
}