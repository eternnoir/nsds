using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsds.RedisDictionaryService
{
    public class RedisDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private string connectionString;
        private ConnectionMultiplexer redis;
        private int databaseNumber;
        private string classTypePrefix;

        public RedisDictionary(string connectionString, int databaseNumber, string classTypePrefix = "TYPE.")
        {
            this.connectionString = connectionString;
            this.redis = ConnectionMultiplexer.Connect(this.connectionString);
            this.databaseNumber = databaseNumber;
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.GetValue(key);
            }

            set
            {
                this.GetValue(key);
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.GetRedisKeys();
                
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            var keyString = key.ToString();
            var typeString = value.GetType().FullName;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if(this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private IDatabase GetRedisDb()
        {
            return this.redis.GetDatabase(this.databaseNumber);
        }

        private TValue GetValue(TKey key)
        {
            if (!this.ContainsKey(key))
            {
                throw new KeyNotFoundException(string.Format("{0} key not found.", key.ToString()));
            }

            // TODO
            return default(TValue);
        }

        private void SetValue(TKey key, TValue value)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
            }

            // TODO Set
        }

        private List<TKey> GetRedisKeys()
        {
            var keylist = new List<TKey>();
            var keys = this.redis.GetServer(this.connectionString).Keys();
            foreach(var key in Keys)
            {
                keylist.Add((TKey)key);
            }
            return keylist;
        }
    }
}
