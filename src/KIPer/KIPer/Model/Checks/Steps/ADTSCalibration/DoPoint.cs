﻿using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.Params;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
{
    class DoPoint : TestStep
    {
        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly double _point;
        private readonly double _tolerance;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private readonly IEthalonChannel _ethalonChannel;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public DoPoint(string name, ADTSModel adts, Parameters param, double point, double tolerance, double rate, PressureUnits unit, IEthalonChannel ethalonChannel, Logger logger)
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

        public string Name { get; private set; }

        public override void Start(EventWaitHandle whEnd)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            var cancel = _cancellationTokenSource.Token;
            if (_adts.SetPressure(_param, _point, _rate, _unit, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] Set point")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetPressurePoint });
                whEnd.Set();
                return;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            EventWaitHandle wh = _param == Parameters.PT ? _adts.WaitPitotSetted() : _adts.WaitPressureSetted();

            while (wh.WaitOne(waitPointPeriod))
            {
                if (cancel.IsCancellationRequested)
                {
                    _adts.StopWaitStatus(wh);
                    whEnd.Set();
                    return;
                }
            }

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            var realValue = _ethalonChannel.GetEthalonValue(_point, cancel);

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            bool correctPoint = Math.Abs(Math.Abs(_point) - Math.Abs(realValue)) <= _tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));
            OnResultUpdated( new EventArgTestResult(new ParameterDescriptor("EthalonValue", _point, ParameterType.RealValue),
                    new ParameterResult(DateTime.Now, realValue)));
            OnResultUpdated( new EventArgTestResult(new ParameterDescriptor("IsCorrect", _point, ParameterType.IsCorrect),
                    new ParameterResult(DateTime.Now, correctPoint)));


            if (_adts.SetActualValue(realValue, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] Can not set real value")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetRealValue });
                whEnd.Set();
                return;
            }

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Точка {0}: Реальное значени {1}({2})",
                    _point, realValue, correctPoint ? "correct" : "incorrect")));
            whEnd.Set();
            return;
        }

        public override bool Stop()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return true;
        }
    }
}