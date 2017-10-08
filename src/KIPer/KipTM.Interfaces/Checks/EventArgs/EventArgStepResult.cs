using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO.Params;

namespace KipTM.Model.Checks
{
    public class EventArgStepResult<T> : EventArgs
    {
        public EventArgStepResult(T results)
        {
            Result = results;
        }

        public T Result { get; private set; }
    }

    public class EventArgStepResultDict : EventArgStepResult<IDictionary<ParameterDescriptor, ParameterResult>>
    {
        public EventArgStepResultDict(ParameterDescriptor descr, ParameterResult res)
            :base(new Dictionary<ParameterDescriptor, ParameterResult>() { { descr, res } })
        {}

        public EventArgStepResultDict(IDictionary<ParameterDescriptor, ParameterResult> results):base(results)
        {}
    }
}
