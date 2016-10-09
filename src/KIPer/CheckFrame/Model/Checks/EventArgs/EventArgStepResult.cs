using System.Collections.Generic;
using ArchiveData.DTO.Params;

namespace CheckFrame.Model.Checks.EventArgs
{
    public class EventArgStepResult : System.EventArgs
    {
        public EventArgStepResult(ParameterDescriptor descr, ParameterResult res)
        {
            Result = new Dictionary<ParameterDescriptor, ParameterResult>() { { descr, res } };
        }

        public EventArgStepResult(IDictionary<ParameterDescriptor, ParameterResult> results)
        {
            Result = results;
        }

        public IDictionary<ParameterDescriptor, ParameterResult> Result { get; private set; }
    }
}
