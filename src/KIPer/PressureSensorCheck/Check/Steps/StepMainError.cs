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
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Data;
using PressureSensorData;
using Tools;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Шаг инициализации поверки датчика давления
    /// </summary>
    internal class StepMainError : TestStepWithBuffer
    {
        /// <summary>
        /// Ключь шага
        /// </summary>
        public const string KeyStep = "MainError";
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly NLog.Logger _logger;
        /// <summary>
        /// Точка проверки
        /// </summary>
        private readonly PressureSensorPoint _point;
        /// <summary>
        /// Результат проверки точки
        /// </summary>
        private PressureSensorPointResult _result = null;
        /// <summary>
        /// Канал работы с пользователем
        /// </summary>
        private readonly IUserChannel _userChannel;
        /// <summary>
        /// Эталонный измеритель давления
        /// </summary>
        private readonly IEthalonChannel _ethalonPressure;
        /// <summary>
        /// Эталонный измеритель напряжения
        /// </summary>
        private readonly IEthalonChannel _ethalonVoltage;

        public StepMainError(PressureSensorPoint point, IUserChannel userChannel, IEthalonChannel ethalonPressure, IEthalonChannel ethalonVoltage, Logger logger)
        {
            Name = $"Проверка основной погрешности на точке {point.PressurePoint} {point.PressureUnit}";
            _point = point;
            _userChannel = userChannel;
            _ethalonPressure = ethalonPressure;
            _ethalonVoltage = ethalonVoltage;
            _logger = logger;
        }

        /// <summary>
        /// Выполнить шаг
        /// </summary>
        /// <param name="cancel"></param>
        public override void Start(CancellationToken cancel)
        {
            OnStarted();
            Log($"Wait set {_point.PressurePoint} {_point.PressureUnit}");
            _userChannel.Message = $"Установите на эталонном источнике давления значение {_point.PressurePoint} {_point.PressureUnit}";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
            var valueVoltage = _ethalonVoltage.GetEthalonValue(_point.VoltagePoint, cancel);
            var valuePressure = _ethalonPressure.GetEthalonValue(_point.PressurePoint, cancel);
            Log($"Received U = {valueVoltage} on V = {valuePressure}");
            _result = new PressureSensorPointResult()
            {
                PressurePoint = _point.PressurePoint,
                PressureUnit = _point.PressureUnit,
                VoltagePoint = _point.VoltagePoint,
                VoltageUnit = _point.VoltageUnit,
                VoltageValue = valueVoltage,
                PressureValue = valuePressure
            };

            if (_buffer != null)
            {
                Log("save result point to buffer");
                PressureSensorResult res;
                if (_buffer.TryResolve<PressureSensorResult>(out res))
                {
                    res.Points.Add(_result);
                }
            }
            OnEnd(new EventArgEnd(KeyStep, true));
        }

        private void Log(string s)
        {
            _logger.With(l => l.Trace(s));
        }
    }
}
