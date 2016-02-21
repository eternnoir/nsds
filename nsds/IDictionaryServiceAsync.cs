using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nsds
{
    public interface IDictionaryServiceAsync
    {
        Task<object> this[string key] { get; set; }
        Task<int> Count { get; }
        Task<ICollection<string>> Keys { get; }
        Task Add(string key, object value);
        Task Add(string key, object value, TimeSpan slidingExpiration);
        Task<bool> ContainsKey(string key);
        Task<bool> Remove(string key);
        Task<object> Get(string key);
        Task<bool> TryGetValue(string key, out object value);
    }
}