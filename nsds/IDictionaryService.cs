using System;
using System.Collections;
using System.Collections.Generic;

namespace nsds
{
    public interface IDictionaryService: ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        string DictionaryServiceId { get; }
        object this[string key] { get; set; }
        int Count { get; }
        ICollection<string> Keys { get; }
        void Add(string key, object value);
        void Add(string key, object value, TimeSpan slidingExpiration);
        object Get(string key);
        bool ContainsKey(string key);
        bool Remove(string key);
        bool TryGetValue(string key, out object value);
    }
}
