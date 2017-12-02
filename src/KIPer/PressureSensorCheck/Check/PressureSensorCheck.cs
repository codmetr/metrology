using System;
using System.Collections.Generic;
using System.Threading;
using CheckFrame.Checks;
using KipTM.Archive;
using KipTM.Interfaces.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check.Steps;
using PressureSensorCheck.Data;
using PressureSensorData;

namespace PressureSensorCheck.Check
{
    public class PressureSensorCheck: CheckBase
    {
        public static string CheckKey = "PressureSensor";
        public static string CheckName = "Поверка датчика давления";
        private SimpleDataBuffer _dataBuffer = new SimpleDataBuffer();

        private IEthalonChannel _pressure;
        private IEthalonChannel _voltage;

        private PressureSensorPointResult _resultPoint = null;
        private PressureSensorResult _result = null;


        public PressureSensorCheck(Logger logger, IEthalonChannel pressure, IEthalonChannel voltage) : base(logger)
        {
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

        public bool FillSteps(PressureSensorConfig pressureConverterConfig)
        {
            var steps = new List<CheckStepConfig>()
            {
                new CheckStepConfig(new StepInit(ChConfig.UsrChannel) { _pointBase= new PressureSensorPoint() { PressurePoint = 760, PressureUnit = "мм рт.ст." } }, true),
            };

            var count = pressureConverterConfig.Points.Count;
            var backStepPoints = new Tuple<PressureSensorPoint, PressureSensorPointResult>[count];
            var i = 0;
            foreach (var point in pressureConverterConfig.Points)
            {
                var step = new StepMainError(point, ChConfig.UsrChannel, _pressure, _voltage, _logger);//TODO: добавить эталоны
                backStepPoints[i] = new Tuple<PressureSensorPoint, PressureSensorPointResult>(point, step.Result);
                step.SetBuffer(_dataBuffer);
                steps.Add(new CheckStepConfig(step, false));
                i++;
            }

            for (; i >= 0; i--)
            {
                var point = backStepPoints[i].Item1;
                var res = backStepPoints[i].Item2;
                var step = new StepMainErrorBack(point, res, ChConfig.UsrChannel, _pressure, _voltage, _logger);
                steps.Add(new CheckStepConfig(step, false));
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
            if(_dataBuffer.TryResolve(out _resultPoint))
                _result.Points.Add(_resultPoint);
            _dataBuffer.Clear();
        }

        protected override void OnEndMethod(EventArgs e)
        {
            _dataBuffer.Clear();
            base.OnEndMethod(e);
        }

        public PressureSensorResult Result { get { return _result; } }
    }
}
