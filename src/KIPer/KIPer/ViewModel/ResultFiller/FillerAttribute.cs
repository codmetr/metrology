using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.ResultFiller
{
    public class FillerAttribute:Attribute
    {
        public FillerAttribute(string typeKey, string resultKey)
        {
            ResultKey = resultKey;
            TypeKey = typeKey;
        }

        public string TypeKey { get; private set; }

        public string ResultKey { get; private set; }
    }
}
