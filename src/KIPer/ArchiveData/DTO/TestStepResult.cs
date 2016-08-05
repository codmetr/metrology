using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO
{
    public class TestStepResult
    {
        private string _checkKey;
        private string _channelKey;
        private string _stepKey;
        private object _result;

        public TestStepResult()
        {
        }

        public TestStepResult(string checkKey, string channelKey, string stepKey, object result)
            : this()
        {
            _channelKey = channelKey;
            _checkKey = checkKey;
            _stepKey = stepKey;
            _result = result;
        }

        public string ChannelKey
        {
            get { return _channelKey; }
            set { _channelKey = value; }
        }

        public string CheckKey
        {
            get { return _checkKey; }
            set { _checkKey = value; }
        }

        public string StepKey
        {
            get { return _stepKey; }
            set { _stepKey = value; }
        }

        public object Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
