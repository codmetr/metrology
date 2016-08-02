using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportService
{
    public class ReportAttribute : Attribute
    {
        private readonly Type _targetType;

        public ReportAttribute(Type targetType)
        {
            _targetType = targetType;
        }

        public Type TargetReportKey
        {
            get { return _targetType; }
        }
    }
}
