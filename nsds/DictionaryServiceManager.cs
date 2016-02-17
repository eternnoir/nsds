using nsds.Exceptions;
using System;
using System.Collections.Generic;

namespace nsds
{
    public sealed class DictionaryServiceManager
    {
        private static Dictionary<string, IDictionaryService> dsDic = new Dictionary<string, IDictionaryService>();
        private static object syncRoot = new object();

        private DictionaryServiceManager()
        {
        }

        public static void AddDictionaryService(string id, IDictionaryService ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException(string.Format("Dictionary Service can not be null."));
            }

            lock (syncRoot)
            {
                dsDic[id] = ds;
            }
        }

        public static IDictionaryService GetDictionaryService(string id)
        {
            lock (syncRoot)
            {
                if (!dsDic.ContainsKey(id))
                {
                    throw new DictionaryServiceNotFoundException(string.Format("ID {0} not found.", id));
                }
                return dsDic[id];
            }
        }

        public static void ReomveDictionaryService(string id)
        {
            lock (syncRoot)
            {
                if (!dsDic.ContainsKey(id))
                {
                    throw new DictionaryServiceNotFoundException(string.Format("ID {0} not found.", id));
                }
                dsDic.Remove(id);
            }
        }
    }
}
