using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace nsds.SqlServerDictionaryService
{
    public class SqlServerDictionaryService : IDictionaryService, IDisposable
    {
        private DbConnection dbc;
        private bool needColseConnection;

        public SqlServerDictionaryService(DbConnection dbConnection, bool needCloseConnection = true)
        {
            this.dbc = dbConnection;
            this.needColseConnection = needCloseConnection;
        }


        public void Dispose()
        {
            this.dbc.Dispose();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        int IDictionaryService.Count { get; }
        public ICollection<string> Keys { get; }
        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        public string DictionaryServiceId { get; }

        public object this[string key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        int ICollection<KeyValuePair<string, object>>.Count { get; }

        public bool IsReadOnly
        {
            get { return false; }
        }

        private void CloseConnection()
        {
            if (this.needColseConnection)
            {
                this.dbc.Close();
            }
        }
    }
}
