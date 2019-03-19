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
        private readonly PressureSensorPointConf _pointConf;
        /// <summary>
        /// Результат проверки точки
        /// </summary>
        private PressureSensorPoint _result = null;
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
        public StepMainError(int index, PressureSensorPointConf pointConf, IUserChannel userChannel,
            IEtalonSourceChannel<Units> ethalonPressureSource, IEtalonChannel etalonPressure, IEtalonChannel etalonVoltage, Logger logger)
        {
            Name = $"Проверка основной погрешности на точке {pointConf.PressurePoint} {pointConf.PressureUnit}";
            _pointConf = pointConf;
            _result = new PressureSensorPoint() { Index = index, Config = _pointConf };
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
            Log($"Wait set {_pointConf.PressurePoint} {_pointConf.PressureUnit}");

            _ethalonPressureSource.SetEtalonValue(_pointConf.PressurePoint, _pointConf.PressureUnit, cancel);
            //_userChannel.Message = $"Установите на эталонном источнике давления значение {_point.PressurePoint} {_point.PressureUnit}, задайте реальное значение давления в графе Pэт и нажмите \"Далее\"";
            //var wh = new ManualResetEvent(false);
            //_userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            //WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
            var valueVoltage = _etalonVoltage.GetEtalonValue(_pointConf.OutPoint, cancel);
            var valuePressure = _etalonPressure.GetEtalonValue(_pointConf.PressurePoint, cancel);
            Log($"Received I = {valueVoltage} on P = {valuePressure}");
            if(_result.Result == null)
                _result.Result = new PressureSensorPointResult();
            _result.Result.PressurePoint = _pointConf.PressurePoint;
            _result.Result.PressureUnit = _pointConf.PressureUnit;
            _result.Result.VoltagePoint = _pointConf.OutPoint;
            _result.Result.VoltageUnit = _pointConf.OutUnit;
            _result.Result.OutPutValue = valueVoltage;
            _result.Result.IsCorrect = Math.Abs(valueVoltage - _pointConf.OutPoint) < _result.Config.Tollerance;
            _result.Result.PressureValue = valuePressure;
            _result.Result.OutPutValueBack = Double.NaN;
            _result.Result.IsCorrectBack = true;

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
        public PressureSensorPoint Result { get { return _result; } }
    }
}
