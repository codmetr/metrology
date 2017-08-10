using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;

namespace PressureSensorCheck.Check.Steps
{
    class StepProbing : TestStep
    {
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;

        public override void Start(CancellationToken cancel)
        {
            var wh = new ManualResetEvent(false);
            _userChannel.Message = "Выполните опробирование. Если все в норме нажмите Да";
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] {wh, cancel.WaitHandle});
            if(cancel.IsCancellationRequested)
                return;

        }
    }
}
