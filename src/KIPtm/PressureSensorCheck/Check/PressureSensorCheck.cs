﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CheckFrame.Checks;
using KipTM.Archive;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Channels;
using PressureSensorCheck.Check.Steps;
using PressureSensorData;

namespace PressureSensorCheck.Check
{
    public class PresSensorCheck: CheckBase
    {
        public static string CheckKey = "PressureSensor";
        public static string CheckName = "Поверка датчика давления";
        private SimpleDataBuffer _dataBuffer = new SimpleDataBuffer();

        private readonly IEthalonSourceChannel<Units> _pressureSrc;
        private readonly IEthalonChannel _pressure;
        private readonly IEthalonChannel _voltage;

        private PressureSensorPointResult _resultPoint = null;
        private PressureSensorResult _result = null;


        public PresSensorCheck(Logger logger, IEthalonSourceChannel<Units> pressureSrc, IEthalonChannel pressure, IEthalonChannel voltage, PressureSensorResult result) : base(logger)
        {
            _result = result;
            _pressureSrc = pressureSrc;
            _pressure = pressure;
            _voltage = voltage;
        }

        public override object GetCustomConfig(IPropertyPool propertyPool)
        {
            throw new NotImplementedException();
        }

        public override bool Init(object customConf)
        {
            return FillSteps(customConf as PressureSensorConfig);//TODO: избавиться от этого метода или придумать как передавать во внутрь канал к GUI
        }

        /// <summary>
        /// Заполнить конфигурацию проверки
        /// </summary>
        /// <param name="pressureConverterConfig"></param>
        /// <returns></returns>
        internal bool FillSteps(PressureSensorConfig pressureConverterConfig)
        {
            var steps = new List<CheckStepConfig>()
            {
                //new CheckStepConfig(new StepInit(ChConfig.UsrChannel) { _pointBase= new PressureSensorPoint() { PressurePoint = 760, PressureUnit = "мм рт.ст." } }, true),
            };

            var count = pressureConverterConfig.Points.Count;
            var backStepPoints = new Tuple<PressureSensorPoint, PressureSensorPointResult>[count];
            var i = 0;
            var presSourceUch = new UChPresSource(ChConfig.UsrChannel);
            foreach (var point in pressureConverterConfig.Points)
            {
                var step = new StepMainError(point, ChConfig.UsrChannel, _pressureSrc?? presSourceUch, _pressure, _voltage, _logger);//TODO: добавить эталоны
                backStepPoints[i] = new Tuple<PressureSensorPoint, PressureSensorPointResult>(point, step.Result);
                step.SetBuffer(_dataBuffer);
                steps.Add(new CheckStepConfig(step, false));
                AttachStep(step);
                i++;
            }

            for (i--; i >= 0; i--)
            {
                var point = backStepPoints[i].Item1;
                var res = backStepPoints[i].Item2;
                var step = new StepMainErrorBack(point, res, ChConfig.UsrChannel, _pressure, _voltage, _logger);
                step.SetBuffer(_dataBuffer);
                steps.Add(new CheckStepConfig(step, false));
                AttachStep(step);
            }
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null)
                        DetachStep(testStep.Step);
                }
            Steps = steps;
            return true;
        }

        protected override bool PrepareCheck(CancellationToken cancel)
        {
            //if (!base.PrepareCheck(cancel))
            //    return false;
            _dataBuffer.Clear();
            return true;
        }

        protected override void StepEnd(object sender, EventArgEnd e)
        {
            if (_dataBuffer.TryResolve(out _resultPoint))
            {
                _result.Points.Add(_resultPoint);
                OnResultUpdated();
            }
            _dataBuffer.Clear();
        }

        protected override void OnEndMethod(EventArgs e)
        {
            _dataBuffer.Clear();
            base.OnEndMethod(e);
        }

        public PressureSensorResult Result { get { return _result; } }

        public event EventHandler ResultUpdated;

        protected virtual void OnResultUpdated()
        {
            ResultUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}