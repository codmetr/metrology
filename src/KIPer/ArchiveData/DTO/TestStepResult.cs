using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO
{
    public class TestStepResult
    {
        private string _stepKey;
        private object _result;

        public TestStepResult()
        {
            
        }

        public TestStepResult(string stepKey, object result):this()
        {
            _stepKey = stepKey;
            _result = result;
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
