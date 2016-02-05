using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsds
{
    public interface IDictionaryService
    {
        string DictionaryServiceId { get; }
        IDictionary<string, object> GetDictionary();
    }
}
