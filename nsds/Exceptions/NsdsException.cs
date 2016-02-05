using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsds.Exceptions
{
    [Serializable]
    public class NsdsException : Exception
    {
        private string errorCode;

        public NsdsException(string msg, string errorCode)
            :base(msg)
        {
            this.errorCode = errorCode;
        }

        public string ErrorCode
        {
            get
            {
                return errorCode;
            }
        }
    }
}
