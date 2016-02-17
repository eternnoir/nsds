using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace nsds.RedisDictionaryService
{
    public class RedisDictionaryEnumerator : IEnumerator<KeyValuePair<string, object>>
    {
        private RedisDictionaryService redisDicService;
        private List<string> keyList;
        private int flag = -1;

        public RedisDictionaryEnumerator(RedisDictionaryService rds)
        {
            this.redisDicService = rds;
            this.keyList = this.redisDicService.Keys.ToList();
        }

        public KeyValuePair<string, object> Current
        {
            get
            {
                if (this.flag == -1 || this.flag > this.keyList.Count)
                {
                    throw new InvalidOperationException();
                }
                var key = this.keyList[flag];
                var obj = this.redisDicService[key];
                var ret = new KeyValuePair<string, object>(key, obj);
                return ret;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (this.flag == -1 || this.flag > this.keyList.Count)
                {
                    throw new InvalidOperationException();
                }
                var key = this.keyList[flag];
                var obj = this.redisDicService[key];
                var ret = new KeyValuePair<string, object>(key, obj);
                return ret;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (this.flag >= this.keyList.Count)
            {
                return false;
            }
            else if (this.flag + 1 >= this.keyList.Count)
            {
                return false;
            }
            else
            {
                this.flag++;
                return true;
            }
        }

        public void Reset()
        {
            this.keyList = this.redisDicService.Keys.ToList();
            this.flag = -1;
        }
    }
}
