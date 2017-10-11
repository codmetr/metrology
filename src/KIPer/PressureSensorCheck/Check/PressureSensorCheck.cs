using System;
using System.Collections.Generic;
using System.Threading;
using ArchiveData.DTO.Params;
using CheckFrame.Checks;
using CheckFrame.Model.Checks.Steps;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check.Steps;
using PressureSensorCheck.Data;
using PressureSensorCheck.Devices;
using PressureSensorData;

namespace PressureSensorCheck.Check
{
    public class PressureSensorCheck: CheckBase
    {
        public static string CheckKey = "PressureSensor";
        public static string CheckName = "Поверка датчика давления";
        private SimpleDataBuffer _dataBuffer = new SimpleDataBuffer();

        private PressureSensorPointResult _resultPoint = null;
        private PressureSensorResult _result = null;

        public PressureSensorCheck(Logger logger) : base(logger)
        {
            
        }

        public override object GetCustomConfig(IPropertyPool propertyPool)
        {
            throw new NotImplementedException();
        }

        public override bool Init(object customConf)
        {
            return FillSteps(customConf as PressureSensorConfig);
        }

        private bool FillSteps(PressureSensorConfig pressureConverterConfig)
        {
            var steps = new List<CheckStepConfig>()
            {
                new CheckStepConfig(new StepInit(), true),
            };

            foreach (var point in pressureConverterConfig.Points)
            {
                var step = new StepMainError(point, _userChannel, null, null, _logger);//TODO: добавить эталоны
                step.SetBuffer(_dataBuffer);
                steps.Add(new CheckStepConfig(step, false));
            }
            Steps = steps;
            return true;
        }

        protected override void OnStartAction(CancellationToken cancel)
        {
            _dataBuffer.Clear();
            base.OnStartAction(cancel);
        }

        public IEnumerable<CheckStepConfig> Steps { get; private set; }

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
    }
}
