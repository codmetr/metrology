using System;
using System.Collections.Generic;
using System.Threading;
using ArchiveData.DTO.Params;
using CheckFrame.Model.Checks.Steps;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check.Steps;
using PressureSensorCheck.Data;
using PressureSensorData;

namespace PressureSensorCheck.Check
{
    public class PressureSensorCheck: CheckFrame.Checks.Check
    {
        public static string CheckKey = "PressureSensor";
        public static string CheckName = "Поверка датчика давления";

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
                //steps.Add(new CheckStepConfig(new StepMainError(point, _userChannel, pressureConverterConfig.ChannelFrom, pressureConverterConfig.ChannelTo, _logger), false));
            }
            Steps = steps;
            return true;
        }

        public IEnumerable<CheckStepConfig> Steps { get; private set; }

        protected override void StepEnd(object sender, EventArgEnd e)
        {
            if (e.Key == StepMainError.KeyStep && _resultPoint != null)
            {
                _result.Points.Add(_resultPoint);
                OnResultUpdated(new EventArgTestStepResult(e.Key, _resultPoint));
                _resultPoint = null;
            }
            else if (e.Key == StepInit.KeyStep)
            {
                //OnResultUpdated(new EventArgTestStepResult(e.Key, _result.CheckTime));
            }
        }

        protected override void SwitchParameter(ParameterDescriptor descriptor, ParameterResult result)
        {
            throw new NotImplementedException();
        }
    }
}
