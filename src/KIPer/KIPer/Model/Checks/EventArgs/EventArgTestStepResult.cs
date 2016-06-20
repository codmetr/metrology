using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO.Params;

namespace KipTM.Model.Checks
{
    public class EventArgTestStepResult
    {
        public EventArgTestStepResult(string key, object res)
        {
            Key = key;
            Result = res;
        }

        public string Key { get; private set; }

        public object Result { get; private set; }
    }
}
