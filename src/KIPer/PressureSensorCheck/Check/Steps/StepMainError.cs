using System.Threading;
using CheckFrame.Checks.Steps;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorData;
using Tools;

namespace PressureSensorCheck.Check.Steps
{
    /// <summary>
    /// Шаг прямого хода поверки датчика давления
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

        /// <summary>
        /// Шаг прямого хода поверки датчика давления
        /// </summary>
        public StepMainError(PressureSensorPoint point, IUserChannel userChannel, IEthalonChannel ethalonPressure, IEthalonChannel ethalonVoltage, Logger logger)
        {
            Name = $"Проверка основной погрешности на точке {point.PressurePoint} {point.PressureUnit}";
            _point = point;
            _result = new PressureSensorPointResult();
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
            Log($"Received U = {valueVoltage} on P = {valuePressure}");
            _result.PressurePoint = _point.PressurePoint;
            _result.PressureUnit = _point.PressureUnit;
            _result.VoltagePoint = _point.VoltagePoint;
            _result.VoltageUnit = _point.VoltageUnit;
            _result.VoltageValue = valueVoltage;
            _result.PressureValue = valuePressure;
            _result.VoltageValueBack = valueVoltage;

            if (_buffer != null)
            {
                Log("save result point to buffer");
                _buffer.Append(_result);
            }
            OnEnd(new EventArgEnd(KeyStep, true));
        }

        private void Log(string s)
        {
            _logger.With(l => l.Trace(s));
        }

        /// <summary>
        /// Результат проверки точки
        /// </summary>
        public PressureSensorPointResult Result { get { return _result; } }
    }
}
