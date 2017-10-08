using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckFrame.Checks.Steps;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;
using PressureSensorCheck.Data;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Финальный шаг проверки
    /// </summary>
    class StepFinish: TestStepBase
    {
        /// <summary>
        /// Базовая точка
        /// </summary>
        private readonly PressureSensorPoint _pointBase;

        /// <summary>
        /// Пользовательский канал
        /// </summary>
        private readonly IUserChannel _userChannel;

        public StepFinish(PressureSensorPoint pointBase, IUserChannel userChannel)
        {
            _pointBase = pointBase;
            _userChannel = userChannel;
        }

        public override void Start(CancellationToken cancel)
        {
            _userChannel.Message = $"Установите на эталонном источнике давления значение {_pointBase.PressurePoint} {_pointBase.PressureUnit}";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
        }
    }
}
