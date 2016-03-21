using System;

namespace KipTM.Model.Params
{
    public class ParameterResult
    {
        private readonly DateTime _timestamp;
        private readonly object _value;

        public ParameterResult(DateTime timestamp, object value)
        {
            _timestamp = timestamp;
            _value = value;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
