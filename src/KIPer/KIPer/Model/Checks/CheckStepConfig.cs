using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public class CheckStepConfig
    {
        private readonly ITestStep _step;
        private readonly bool _mandatory;
        private bool _enabled;

        public CheckStepConfig(ITestStep step, bool mandatory, bool enabled = true)
        {
            _step = step;
            _mandatory = mandatory;
            _enabled = enabled;
        }

        public bool Mandatory
        {
            get { return _mandatory; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public ITestStep Step
        {
            get { return _step; }
        }
    }
}
