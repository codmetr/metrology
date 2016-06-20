using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using ArchiveData.DTO.Params;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
{
    class DoPointStep : TestStep, IStoppedOnPoint, ISettedEthalonChannel
    {
        public const string KeyPressure = "Pressure";

        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly double _point;
        private readonly double _tolerance;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private IEthalonChannel _ethalonChannel;
        private readonly NLog.Logger _logger;
        private ManualResetEvent _setCurrentValueAsPoint = new ManualResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource;

        public DoPointStep(string name, ADTSModel adts, Parameters param, double point, double tolerance, double rate, PressureUnits unit, IEthalonChannel ethalonChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _tolerance = tolerance;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            _ethalonChannel = ethalonChannel;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Start(EventWaitHandle whEnd)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            var cancel = _cancellationTokenSource.Token;
            var point = _point;

            OnStarted();
            // Установка единиц измерений
            if (!DoOne(whEnd, cancel, () => _adts.SetPressureUnit(_unit, cancel), "[ERROR] Set unit for point"))
            {
                return;
            }
            // Установить скорость
            if (!DoOne(whEnd, cancel, () => _adts.SetRate(_param, _rate, cancel), "[ERROR] Set rate for point"))
            {
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // Установить цель для ADST
            if (!DoOne(whEnd, cancel, () => _adts.SetParameter(_param, point, cancel), "[ERROR] Set rate for point"))
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
                    whEnd.Set();
                    OnEnd(new EventArgEnd(false));
                    return;
                }
                exitIndex = WaitHandle.WaitAny(whArray, waitPointPeriod);
            }
            if (exitIndex == 1)
            {
                _adts.StopWaitStatus(wh);
                point = _param == Parameters.PT ? _adts.Pitot.GetValueOrDefault(_point) : _adts.Pressure.GetValueOrDefault(_point);
                _adts.SetParameter(_param, point, cancel);
            }

            if (IsCancel(whEnd, cancel))
            {
                return;
            }

            // Получить эталонное значение
            var realValue = _ethalonChannel.GetEthalonValue(point, cancel);
            if (IsCancel(whEnd, cancel))
            {
                return;
            }

            // Расчитать погрешность и зафиксировать реультата
            bool correctPoint = Math.Abs(Math.Abs(point) - Math.Abs(realValue)) <= _tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.RealValue),
                    new ParameterResult(DateTime.Now, realValue)));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.IsCorrect),
                    new ParameterResult(DateTime.Now, correctPoint)));
            if (IsCancel(whEnd, cancel))
            {
                return;
            }

            // Передача ADTS реального значения
            if (!DoOne(whEnd, cancel, () => _adts.SetActualValue(realValue, cancel), "[ERROR] Can not set real value"))
            {
                return;
            }

            // Сдвинуть прогресс
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Точка {0}: Реальное значени {1}({2})",
                    point, realValue, correctPoint ? "correct" : "incorrect")));
            whEnd.Set();
            OnEnd(new EventArgEnd(true));
            return;
        }

        public override bool Stop()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return true;
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
        /// Обертка для выполнения подшага
        /// </summary>
        /// <param name="whEnd"></param>
        /// <param name="cancel"></param>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool DoOne(EventWaitHandle whEnd, CancellationToken cancel, Func<bool> func, string errorMessage)
        {
            if (!func())
            {
                _logger.With(l => l.Trace(errorMessage));
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return false;
            }
            if (IsCancel(whEnd, cancel))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обертка для проверки на "отмену" операции
        /// </summary>
        /// <param name="whEnd"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private bool IsCancel(EventWaitHandle whEnd, CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return true;
            }
            return false;
        }

    }
}
