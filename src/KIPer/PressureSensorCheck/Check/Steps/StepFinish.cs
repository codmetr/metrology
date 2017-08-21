using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;

namespace PressureSensorCheck.Check.Steps
{
    class StepFinish: TestStep
    {
        private IUserChannel _userChannel;
        public override void Start(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
