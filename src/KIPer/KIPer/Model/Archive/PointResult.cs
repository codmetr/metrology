using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Archive
{
    public class PointResult
    {
        private readonly DateTime _timestamp;
        private readonly PointDescriptor _descriptor;
        private readonly object _value;
        private readonly bool _isCorrect;

        public PointResult(DateTime timestamp, PointDescriptor descriptor, object value, bool isCorrect)
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

        public PointDescriptor Descriptor
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
