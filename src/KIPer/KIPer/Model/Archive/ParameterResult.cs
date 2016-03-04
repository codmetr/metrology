using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Archive
{
    public class ParameterResult
    {
        private readonly DateTime _timestamp;

        public ParameterResult(DateTime timestamp)
        {
            _timestamp = timestamp;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }
    }
}
