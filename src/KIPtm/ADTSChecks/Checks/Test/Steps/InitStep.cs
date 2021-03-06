﻿using System;
using System.Threading;
using ADTS;
using ADTSChecks.Devices;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Checks.Steps;
using KipTM.Model.Checks;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSTest
{
    class InitStep : TestStep
    {
        public const string KeyStep = "InitStep";
        public const string KeyCalibDate = "CalibDate";

        private readonly ADTSModel _adts;
        private readonly ChannelDescriptor _calibChan;
        private readonly NLog.Logger _logger;

        public InitStep(string name, ADTSModel adts, ChannelDescriptor calibChan, Logger logger)
        {
            Name = name;
            _adts = adts;
            _calibChan = calibChan;
            _logger = logger;
        }

        public override void Start(CancellationToken cancel)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            DateTime testDate = DateTime.Now;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS test by channel {0}", _calibChan.Name)));
            OnProgressChanged(new EventArgProgress(0, "Запуск Поверки"));
            if (!_adts.SetState(State.Control, cancel))
            {
                if(!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] set state {0}", State.Control)));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            // Дождаться установки параметра или примененения текущей точки как целевой
            EventWaitHandle wh = _adts.WaitControlSetted();
            while (!wh.WaitOne(waitPointPeriod))
            {
                if (cancel.IsCancellationRequested)
                {
                    _adts.StopWaitState(wh);
                    OnEnd(new EventArgEnd(KeyStep, false));
                    return;
                }
            }

            OnResultUpdated(new EventArgStepResultDict(new ParameterDescriptor(KeyCalibDate, null, ParameterType.Metadata), new ParameterResult(DateTime.Now, testDate)));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Поверка запущена (Дата: {0})", testDate.ToString())));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }
    }
}
