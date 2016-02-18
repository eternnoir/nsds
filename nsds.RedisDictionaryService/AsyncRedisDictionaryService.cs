namespace nsds.RedisDictionaryService
{
    #region DEPENDENCY

    using System;
    using System.Collections.Generic;

    #endregion

    public class AsyncRedisDictionaryService
    {
        private readonly RedisDictionaryService redisDictionaryService;

        public AsyncRedisDictionaryService(
            string connectionString, 
            int databaseNumber = 0, 
            int timeDatabaseNumber = 1, 
            bool AllowAdmin = false)
        {
            this.redisDictionaryService = new RedisDictionaryService(
                connectionString, 
                databaseNumber, 
                timeDatabaseNumber, 
                AllowAdmin);
        }

        public AsyncFuture<int> Count
        {
            get
            {
                return DoAsync(() => this.redisDictionaryService.Count);
            }
        }

        public AsyncFuture<ICollection<string>> Keys
        {
            get
            {
                return DoAsync(() => this.redisDictionaryService.Keys);
            }
        }

        public AsyncFuture<object> this[string key]
        {
            get
            {
                return DoAsync(() => this.redisDictionaryService[key]);
            }

            set
            {
                DoAsync(() => this.redisDictionaryService[key] = value);
            }
        }

        public AsyncFuture<Void> Add(string key, object value, TimeSpan slidingExpiration)
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.Add(key, value, slidingExpiration);
                        return Void.Const;
                    });
        }

        public AsyncFuture<Void> Add(KeyValuePair<string, object> item)
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.Add(item);
                        return Void.Const;
                    });
        }

        public AsyncFuture<Void> Add(string key, object value)
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.Add(key, value);
                        return Void.Const;
                    });
        }

        public AsyncFuture<Void> Clear()
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.Clear();
                        return Void.Const;
                    });
        }

        public AsyncFuture<bool> Contains(KeyValuePair<string, object> item)
        {
            return DoAsync(() => this.redisDictionaryService.Contains(item));
        }

        public AsyncFuture<bool> ContainsKey(string key)
        {
            return DoAsync(() => this.redisDictionaryService.ContainsKey(key));
        }

        public AsyncFuture<Void> CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.CopyTo(array, arrayIndex);
                        return Void.Const;
                    });
        }

        public AsyncFuture<Void> Dispose()
        {
            return DoAsync(
                () =>
                    {
                        this.redisDictionaryService.Dispose();
                        return Void.Const;
                    });
        }

        public AsyncFuture<bool> Remove(KeyValuePair<string, object> item)
        {
            return DoAsync(() => this.redisDictionaryService.Remove(item));
        }

        public AsyncFuture<bool> Remove(string key)
        {
            return DoAsync(() => this.redisDictionaryService.Remove(key));
        }

        public AsyncFuture<object> Get(string key)
        {
            return DoAsync(
                () =>
                    {
                        object val;
                        this.redisDictionaryService.TryGetValue(key, out val);
                        return val;
                    });
        }

        private static AsyncFuture<R> DoAsync<R>(AsyncFuture<R>.AsyncTask task)
        {
            return AsyncFuture<R>.Execute(task);
        }
    }
}