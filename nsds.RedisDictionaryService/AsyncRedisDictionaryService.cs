namespace nsds.RedisDictionaryService
{
    #region DEPENDENCY

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// [TODO] EXCEPTION
    /// </summary>
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
                return new AsyncFuture<int>(() => this.redisDictionaryService.Count);
            }
        }

        public AsyncFuture<ICollection<string>> Keys
        {
            get
            {
                return new AsyncFuture<ICollection<string>>(() => this.redisDictionaryService.Keys);
            }
        }

        public AsyncFuture<bool> Add(string key, object value, TimeSpan slidingExpiration)
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.Add(key, value, slidingExpiration);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Add(KeyValuePair<string, object> item)
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.Add(item);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Add(string key, object value)
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.Add(key, value);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Clear()
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.Clear();
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Contains(KeyValuePair<string, object> item)
        {
            return new AsyncFuture<bool>(() => this.redisDictionaryService.Contains(item));
        }

        public AsyncFuture<bool> ContainsKey(string key)
        {
            return new AsyncFuture<bool>(() => this.redisDictionaryService.ContainsKey(key));
        }

        public AsyncFuture<bool> CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.CopyTo(array, arrayIndex);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Dispose()
        {
            return new AsyncFuture<bool>(
                () =>
                    {
                        try
                        {
                            this.redisDictionaryService.Dispose();
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });
        }

        public AsyncFuture<bool> Remove(KeyValuePair<string, object> item)
        {
            return new AsyncFuture<bool>(() => this.redisDictionaryService.Remove(item));
        }

        public AsyncFuture<bool> Remove(string key)
        {
            return new AsyncFuture<bool>(() => this.redisDictionaryService.Remove(key));
        }

        public AsyncFuture<object> Get(string key)
        {
            return new AsyncFuture<object>(
                () =>
                    {
                        object val;
                        this.redisDictionaryService.TryGetValue(key, out val);
                        return val;
                    });
        }
    }
}