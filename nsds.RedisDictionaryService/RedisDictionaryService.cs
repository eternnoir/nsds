using nsds.Exceptions;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace nsds.RedisDictionaryService
{
    public class RedisDictionaryService : IDictionaryService
    {
        private const string dictionaryServiceId = "RedisDictionaryService";
        private string connectionString;
        private ConnectionMultiplexer redis;
        private int databaseNumber;

        public RedisDictionaryService(string connectionString, int databaseNumber = 0, bool AllowAdmin = false)
        {
            this.connectionString = connectionString;
            this.databaseNumber = databaseNumber;
            var options = ConfigurationOptions.Parse(this.connectionString);
            options.AllowAdmin = AllowAdmin;
            this.redis = ConnectionMultiplexer.Connect(options);
        }

        public object this[string key]
        {
            get
            {
                return this.GetValue(key);
            }

            set
            {
                this.SetValue(key, value);
            }
        }

        public int Count
        {
            get
            {
                return this.GetRedisKeys().Count;
            }
        }

        public string DictionaryServiceId
        {
            get
            {
                return this.DictionaryServiceId;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.GetRedisKeys();
            }
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

        public void Clear()
        {
            var server = redis.GetServer(this.connectionString);
            server.FlushDatabase(this.databaseNumber);
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

        private IDatabase GetRedisDb()
        {
            return this.redis.GetDatabase(this.databaseNumber);
        }

        private object GetValue(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new KeyNotFoundException(string.Format("{0} key not found.", key.ToString()));
            }
            var db = this.GetRedisDb();
            byte[] valueBa = db.StringGet(key);
            return this.DeserializeObject(valueBa);
        }


        private void SetValue(string key, object value)
        {
            var typeString = value.GetType().FullName;
            var db = this.GetRedisDb();
            db.StringSet(key, this.SerializeObject(value));
        }

        private List<string> GetRedisKeys()
        {
            var keylist = new List<string>();
            foreach (var key in this.redis.GetServer(this.connectionString).Keys(pattern: "*",database: this.databaseNumber))
            {
                keylist.Add((string)key);
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
