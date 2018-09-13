using System;
using System.Threading;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSData;
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
    public class DoPointStep : TestStepWithBuffer, IStoppedOnPoint, ISettedEtalonChannel
    {
        public const string KeyStep = "DoPointStep";
        public const string KeyPressure = "Pressure";

        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly ADTSPoint _point;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private IEtalonChannel EtalonChannel;
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private AdtsPointResult _result;
        private ManualResetEvent _setCurrentValueAsPoint = new ManualResetEvent(false);

        public DoPointStep(
            string name, ADTSModel adts, Parameters param, ADTSPoint point, double rate,
            PressureUnits unit, IEtalonChannel etalonChannel, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            EtalonChannel = etalonChannel;
            _userChannel = userChannel;
            _result = new AdtsPointResult();
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
            var realValue = EtalonChannel.GetEtalonValue(point, cancel);
            if (IsCancel(cancel))
            {
                return;
            }

            // Расчитать погрешность и зафиксировать реультата
            bool correctPoint = Math.Abs(Math.Abs(point) - Math.Abs(realValue)) <= _point.Tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));

            _result.Point = point;
            _result.Tolerance = _point.Tolerance;
            _result.RealValue = realValue;
            _result.Unit = _unit.ToStr();
            _result.Error = Math.Abs(point) - Math.Abs(realValue);
            _result.IsCorrect = correctPoint;

            if (_buffer != null)
            {
                _logger.With(l => l.Trace("save result point to buffer"));
                _buffer.Append(_result);
            }

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
            OnProgressChanged(new EventArgProgress(100, string.Format("Точка {0}: Реальное значени {1}({2})",
                    point, realValue, correctPoint ? "correct" : "incorrect")));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }

        public void SetEtalonChannel(IEtalonChannel ehalon)
        {
            EtalonChannel = ehalon;
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
