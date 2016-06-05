﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks.Steps;
using KipTM.Model.Checks.Steps.ADTSCalibration;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSCheckMethod : ADTSMethodBase
    {
        public static string Key = "Калибровка ADTS";

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        public const string KeyChannel = "Channel";

        private ITestStep _currenTestStep = null;
        private readonly object _currenTestStepLocker = new object();

        public ADTSCheckMethod(NLog.Logger logger) : base(logger)
        {
            MethodName = "Калибровка ADTS";
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(IPropertyPool propertyes)
        {
            var points = propertyes.GetProperty<List<ADTSChechPoint>>(ADTSCheckMethod.KeyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(ADTSCheckMethod.KeyChannel);
            var rate = propertyes.GetProperty<double>(ADTSCheckMethod.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(ADTSCheckMethod.KeyUnit);
            return Init(new ADTSMethodParameters(channel, points, rate, unit));
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(ADTSMethodParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));

            _calibChan = parameters.CalibChannel;

            //if (_userChannel == null)
            //    throw new NullReferenceException("\"UserChannel\" not fount in parameters as IUserChannel");

            var steps = new List<ITestStep>();

            // добавление шага инициализации
            ITestStep step = new Init("Инициализация калибровки", _adts, _calibChan, _logger);
            steps.Add(step);
            step.ResultUpdated += StepResultUpdated;

            // добавление шага прохождения точек
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            foreach (var point in parameters.Points)
            {
                step = new DoPoint(string.Format("Калибровка точки {0}", point.Pressure), _adts, param, point.Pressure, point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _logger);
                step.ResultUpdated += StepResultUpdated;
                steps.Add(step);
            }

            // добавление шага подтверждения калибровки
            step = new Finish("Подтверждение калибровки", _adts, _userChannel, _logger);
            steps.Add(step);
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null) testStep.ResultUpdated -= StepResultUpdated;
                }
            Steps = steps;
            return true;
        }

        /// <summary>
        /// Вызывается перед запуском шага
        /// </summary>
        /// <param name="step"></param>
        protected override void PrepareStartStep(ITestStep step)
        {
            lock (_currenTestStepLocker)
            {
                _currenTestStep = step;
            }
        }

        /// <summary>
        /// Вызывается после завершения шага
        /// </summary>
        /// <param name="step"></param>
        protected override void AfterEndStep(ITestStep step)
        {
            lock (_currenTestStepLocker)
            {
                _currenTestStep = null;
            }
        }

        public void SetCurrentValueAsPoint()
        {
            DoPoint pointstep;
            lock (_currenTestStepLocker)
            {
                pointstep = _currenTestStep as DoPoint;
            }
            if (pointstep == null)
                return;
            pointstep.SetCurrentValueAsPoint();
        }
    }
}
