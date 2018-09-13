using System;
using System.Threading;
using CheckFrame.Checks.Steps;
using KipTM.Interfaces;
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
        /// Эталонный источника давления
        /// </summary>
        private readonly IEtalonSourceChannel<Units> _ethalonPressureSource;
        /// <summary>
        /// Эталонный измеритель давления
        /// </summary>
        private readonly IEtalonChannel _etalonPressure;
        /// <summary>
        /// Эталонный измеритель напряжения
        /// </summary>
        private readonly IEtalonChannel _etalonVoltage;

        /// <summary>
        /// Шаг прямого хода поверки датчика давления
        /// </summary>
        public StepMainError(PressureSensorPoint point, IUserChannel userChannel,
            IEtalonSourceChannel<Units> ethalonPressureSource, IEtalonChannel etalonPressure, IEtalonChannel etalonVoltage, Logger logger)
        {
            Name = $"Проверка основной погрешности на точке {point.PressurePoint} {point.PressureUnit}";
            _point = point;
            _result = new PressureSensorPointResult();
            _userChannel = userChannel;
            _ethalonPressureSource = ethalonPressureSource;
            _etalonPressure = etalonPressure;
            _etalonVoltage = etalonVoltage;
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

            _ethalonPressureSource.SetEtalonValue(_point.PressurePoint, _point.PressureUnit, cancel);
            //_userChannel.Message = $"Установите на эталонном источнике давления значение {_point.PressurePoint} {_point.PressureUnit}, задайте реальное значение давления в графе Pэт и нажмите \"Далее\"";
            //var wh = new ManualResetEvent(false);
            //_userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            //WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
            var valueVoltage = _etalonVoltage.GetEtalonValue(_point.OutPoint, cancel);
            var valuePressure = _etalonPressure.GetEtalonValue(_point.PressurePoint, cancel);
            Log($"Received I = {valueVoltage} on P = {valuePressure}");
            _result.PressurePoint = _point.PressurePoint;
            _result.PressureUnit = _point.PressureUnit;
            _result.VoltagePoint = _point.OutPoint;
            _result.VoltageUnit = _point.OutUnit;
            _result.VoltageValue = valueVoltage;
            _result.PressureValue = valuePressure;
            _result.VoltageValueBack = Double.NaN;

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
