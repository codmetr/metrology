using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckFrame.Checks.Steps;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;
using PressureSensorData;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Шаг инициализации поверки датчика давления
    /// </summary>
    internal class StepInit : TestStepBase
    {
        public static string KeyStep = "Init";

        /// <summary>
        /// Базовая точка
        /// </summary>
        public PressureSensorPointConf PointConfBase;

        /// <summary>
        /// Пользовательский канал
        /// </summary>
        private readonly IUserChannel _userChannel;

        public StepInit(IUserChannel userChannel)
        {
            _userChannel = userChannel;
        }

        public override void Start(CancellationToken cancel)
        {
            _userChannel.Message = $"Установите на эталонном источнике давления значение {PointConfBase.PressurePoint} {PointConfBase.PressureUnit}";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] {wh, cancel.WaitHandle});
            if(cancel.IsCancellationRequested)
                return;
        }
    }
}
