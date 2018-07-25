using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckFrame.Checks.Steps;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using PressureSensorData;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Финальный шаг проверки
    /// </summary>
    class StepFinish: TestStepBase, IFinalizeStep
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
            var msg = $"Установите на эталонном источнике давления значение {_pointBase.PressurePoint} {_pointBase.PressureUnit}";
            _userChannel.ShowModal("Нормализация давления", msg, cancel);
            if (cancel.IsCancellationRequested)
                return;
        }
    }
}
