using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nsds
{
    public interface IDictionaryServiceAsync
    {
        Task AddAsync(string key, object value);
        Task AddAsync(string key, object value, TimeSpan slidingExpiration);
        Task<bool> ContainsKeyAsync(string key);
        Task<bool> RemoveAsync(string key);
        Task<object> GetAsync(string key);
        Task ClearAsync();
    }
}