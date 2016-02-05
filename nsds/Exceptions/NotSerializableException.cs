using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsds.Exceptions
{
    public class NotSerializableException : NsdsException 
    {
        public NotSerializableException(string msg)
            :base(msg, "11")
        {
        }
    }
}
