using System;
using System.Threading;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ArchiveData.DTO.Params;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Checks.Steps;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSCalibration
{
    public class DoPointStep : TestStep, IStoppedOnPoint, ISettedEthalonChannel
    {
        public const string KeyStep = "DoPointStep";
        public const string KeyPressure = "Pressure";

        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly ADTSPoint _point;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private IEthalonChannel _ethalonChannel;
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private ManualResetEvent _setCurrentValueAsPoint = new ManualResetEvent(false);

        public DoPointStep(
            string name, ADTSModel adts, Parameters param, ADTSPoint point, double rate,
            PressureUnits unit, IEthalonChannel ethalonChannel, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            _ethalonChannel = ethalonChannel;
            _userChannel = userChannel;
        }

        public override void Start(CancellationToken cancel)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            var point = _point.Pressure;

            OnStarted();
            // Установка единиц измерений
            if (!DoOne(cancel, () => _adts.SetPressureUnit(_unit, cancel), "[ERROR] Set unit for point"))
            {
                return;
            }
            // Установить скорость
            if (!DoOne(cancel, () => _adts.SetRate(_param, _rate, cancel), "[ERROR] Set rate for point"))
            {
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // Установить цель для ADST
            if (!DoOne(cancel, () => _adts.SetParameter(_param, point, cancel), "[ERROR] Set rate for point"))
            {
                return;
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            EventWaitHandle wh = _param == Parameters.PT ? _adts.WaitPitotSetted() : _adts.WaitPressureSetted();
            var whArray = new WaitHandle[] { wh, _setCurrentValueAsPoint };
            int exitIndex = WaitHandle.WaitAny(whArray, waitPointPeriod);
            while (exitIndex < 0)
            {
                if (cancel.IsCancellationRequested)
                {
                    _adts.StopWaitStatus(wh);
                    OnEnd(new EventArgEnd(KeyStep, false));
                    return;
                }
                exitIndex = WaitHandle.WaitAny(whArray, waitPointPeriod);
            }
            if (exitIndex == 1)
            {
                _adts.StopWaitStatus(wh);
                point = _param == Parameters.PT ? _adts.Pitot.GetValueOrDefault(point) : _adts.Pressure.GetValueOrDefault(point);
                _adts.SetParameter(_param, point, cancel);
            }

            if (IsCancel(cancel))
            {
                return;
            }

            // Получить эталонное значение
            var realValue = _ethalonChannel.GetEthalonValue(point, cancel);
            if (IsCancel(cancel))
            {
                return;
            }

            // Расчитать погрешность и зафиксировать реультата
            bool correctPoint = Math.Abs(Math.Abs(point) - Math.Abs(realValue)) <= _point.Tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.RealValue),
                    new ParameterResult(DateTime.Now, realValue)));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.Unit),
                    new ParameterResult(DateTime.Now, _unit.ToStr())));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.IsCorrect),
                    new ParameterResult(DateTime.Now, correctPoint)));
            if (IsCancel(cancel))
            {
                return;
            }

            // Передача ADTS реального значения
            if (!DoOne(cancel, () => _adts.SetActualValue(realValue, cancel), "[ERROR] Can not set real value"))
            {
                return;
            }

            // Сдвинуть прогресс
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Точка {0}: Реальное значени {1}({2})",
                    point, realValue, correctPoint ? "correct" : "incorrect")));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }

        public void SetEthalonChannel(IEthalonChannel ehalon)
        {
            _ethalonChannel = ehalon;
        }


        /// <summary>
        /// Установить теущее значение как точку
        /// </summary>
        public void SetCurrentValueAsPoint()
        {
            _setCurrentValueAsPoint.Set();
        }

        /// <summary>
        /// Ключевая точка
        /// </summary>
        public double Point { get { return _point.Pressure; } }

        /// <summary>
        /// Ключевая точка
        /// </summary>
        public string Unit { get { return _unit.ToStr(); } }

        /// <summary>
        /// Допуск на ключевой точке
        /// </summary>
        public double Tolerance { get { return _point.Tolerance; } }

        /// <summary>
        /// Обертка для выполнения подшага
        /// </summary>
        /// <param name="whEnd"></param>
        /// <param name="cancel"></param>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool DoOne(CancellationToken cancel, Func<bool> func, string errorMessage)
        {
            if (!func())
            {
                _logger.With(l => l.Trace(errorMessage));
                OnEnd(new EventArgEnd(KeyStep, false));
                return false;
            }
            if (IsCancel(cancel))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обертка для проверки на "отмену" операции
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private bool IsCancel(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return true;
            }
            return false;
        }
    }
}
