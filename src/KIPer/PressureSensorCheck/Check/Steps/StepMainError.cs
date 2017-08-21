using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks.Steps;
using KipTM.Model.Channels;
using PressureSensorCheck.Data;
using PressureSensorData;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Шаг инициализации поверки датчика давления
    /// </summary>
    internal class StepMainError : TestStep
    {
        /// <summary>
        /// Точка проверки
        /// </summary>
        public PressureConverterPoint _point;

        /// <summary>
        /// Результат проверки точки
        /// </summary>
        public PressureSensorPointResult _result = null;

        /// <summary>
        /// Канал работы с пользователем
        /// </summary>
        private IUserChannel _userChannel;
        /// <summary>
        /// Эталонный измеритель давления
        /// </summary>
        private IEthalonChannel _ethalonPressure;
        /// <summary>
        /// Эталонный измеритель напряжения
        /// </summary>
        private IEthalonChannel _ethalonVoltage;

        /// <summary>
        /// Выполнить шаг
        /// </summary>
        /// <param name="cancel"></param>
        public override void Start(CancellationToken cancel)
        {
            _userChannel.Message = $"Установите на эталонном источнике давления значение {_point.PressurePoint} {_point.PressureUnit}";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
            var valueVoltage = _ethalonVoltage.GetEthalonValue(_point.VoltagePoint, cancel);
            var valuePressure = _ethalonVoltage.GetEthalonValue(_point.PressurePoint, cancel);
            _result = new PressureSensorPointResult()
            {
                PressurePoint = _point.PressurePoint,
                PressureUnit = _point.PressureUnit,
                VoltagePoint = _point.VoltagePoint,
                VoltageUnit = _point.VoltageUnit,
                VoltageValue = valueVoltage,
                PressureValue = valuePressure
            };
        }
    }
}
