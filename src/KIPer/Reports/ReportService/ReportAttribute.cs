using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportService
{
    public class ReportAttribute : Attribute
    {
        private readonly string _targetKey = null;

        public ReportAttribute(string targetKey)
        {
            _targetKey = targetKey;
        }

        public string ReportKey
        {
            get { return _targetKey; }
        }
    }
}
