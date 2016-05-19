using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO.Params;

namespace KipTM.Model.Checks
{
    public class EventArgTestResult
    {
        public EventArgTestResult(ParameterDescriptor descr, ParameterResult res)
        {
            Result = new Dictionary<ParameterDescriptor, ParameterResult>() { { descr, res } };
        }

        public EventArgTestResult(IDictionary<ParameterDescriptor, ParameterResult> results)
        {
            Result = results;
        }

        public IDictionary<ParameterDescriptor, ParameterResult> Result { get; private set; }
    }
}
