using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Devices;

namespace KipTM.Model.Archive
{
    public class TestResult
    {
        public string User { get; set; }

        public string Note { get; set; }

        public DateTime Timestamp { get; set; }

        public DeviceDescriptor TargetDevice { get; set; }

        public IEnumerable<DeviceDescriptor> Etalon { get; set; }

        public IEnumerable<ParameterResult> Parameters { get; set; } 
    }
}
