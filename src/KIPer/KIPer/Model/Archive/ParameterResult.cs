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
        private readonly ParameterDescriptor _descriptor;
        private readonly object _value;
        private bool _isCorrect;

        public ParameterResult(DateTime timestamp, ParameterDescriptor descriptor, object value, bool isCorrect)
        {
            _timestamp = timestamp;
            _descriptor = descriptor;
            _value = value;
            _isCorrect = isCorrect;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public ParameterDescriptor Descriptor
        {
            get { return _descriptor; }
        }

        public object Value
        {
            get { return _value; }
        }

        public bool IsCorrect
        {
            get { return _isCorrect; }
        }
    }
}
