using System;
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
    /// Шаг обратного хода поверки датчика давления
    /// </summary>
    internal class StepMainErrorBack : TestStepWithBuffer
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
        /// Эталонный измеритель давления
        /// </summary>
        private readonly IEtalonChannel _etalonPressure;
        /// <summary>
        /// Эталонный измеритель напряжения
        /// </summary>
        private readonly IEtalonChannel _etalonVoltage;

        /// <summary>
        /// Шаг обратного хода поверки датчика давления
        /// </summary>
        public StepMainErrorBack(PressureSensorPoint result, IUserChannel userChannel, IEtalonChannel etalonPressure, IEtalonChannel etalonVoltage, Logger logger)
        {
            Name = $"Проверка основной погрешности обратного хода на точке {result.Config.PressurePoint} {result.Config.PressureUnit} и нажмите \"Далее\"";
            _pointConf = result.Config;
            _result = result;
            _userChannel = userChannel;
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
            _userChannel.Message = $"Установите на эталонном источнике давления значение {_pointConf.PressurePoint} {_pointConf.PressureUnit}";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            if (cancel.IsCancellationRequested)
                return;
            var valueVoltage = _etalonVoltage.GetEtalonValue(_pointConf.OutPoint, cancel);
            var valuePressure = _etalonPressure.GetEtalonValue(_pointConf.PressurePoint, cancel);
            Log($"Received I = {valueVoltage} on P = {valuePressure}");
            _result.Result.OutPutValueBack = valueVoltage;
            _result.Result.IsCorrectBack = Math.Abs(valueVoltage - _result.Config.OutPoint) < _result.Config.Tollerance;

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
    }
}
