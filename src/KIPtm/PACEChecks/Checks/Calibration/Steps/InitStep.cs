using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Checks.Steps;
using PACEChecks.Devices;

namespace PACEChecks.Checks.Calibration.Steps
{
    public class InitStep : TestStep
    {
        public const string KeyStep = "InitStep";

        //private PACE1000Model _pace1000;


        public override void Start(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
