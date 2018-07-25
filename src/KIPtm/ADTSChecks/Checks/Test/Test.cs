using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using ADTSChecks.Checks;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSChecks.Model.Steps.ADTSTest;
using ADTSData;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Archive;
using CheckFrame.Checks;
using KipTM.Archive;
using KipTM.Model.Checks;
using Tools;

namespace ADTSChecks.Model.Checks
{
    public class Test : CheckBaseADTS
    {
        public const string key = KeysDic.Test;

        //public const string KeyPropertyPoints = "Points";
        //public const string KeyPropertyRate = "Rate";
        //public const string KeyPropertyUnit = "Unit";
        //public const string KeyPropertyChannel = "Channel";
        public PressureUnits _unit;

        private AdtsTestResults _result = null;
        private AdtsPointResult _resultPoint = null;

        private SimpleDataBuffer _dataBuffer = new SimpleDataBuffer();


        public Test(NLog.Logger logger)
            : base(logger)
        {
            base.Key = Test.key;
            MethodName = "Поверка ADTS";
            _result = new AdtsTestResults();
            _resultPoint = new AdtsPointResult();
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(object customConf)
        {
            return FillSteps(customConf as ADTSParameters);
        }

        public override object GetCustomConfig(IPropertyPool propertyes)
        {
            //var propertyes = propertyPool.ByKey(ChannelKey);
            var points = propertyes.GetProperty<List<ADTSPoint>>(KeyPoints);
            var channel = propertyes.GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
            var rate = propertyes.GetProperty<double>(KeyRate);
            _unit = propertyes.GetProperty<PressureUnits>(KeyUnit);
            return new ADTSParameters(channel, points, rate, _unit);
        }

        /// <summary>
        /// Заполнение списка шагов 
        /// </summary>
        /// <returns></returns>
        public bool FillSteps(ADTSParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSTestMethodic"));

            _parameters = parameters;
            ChConfig.Channel = parameters.CalibChannel;

            //if (_userChannel == null)
            //    throw new NullReferenceException("\"UserChannel\" not fount in parameters as IUserChannel");

            var steps = new List<CheckStepConfig>();

            // добавление шага инициализации
            CheckStepConfig step = new CheckStepConfig(new InitStep("Инициализация поверки", _adts, ChConfig.Channel, _logger), true);
            AttachStep(step.Step);
            steps.Add(step);

            // добавление шагов прохождения точек
            Parameters param;
            if (ChConfig.Channel.Name == ADTSModel.Ps)
                param = Parameters.PS;
            else if (ChConfig.Channel.Name == ADTSModel.Pt)
                param = Parameters.PT;
            else param = Parameters.PS;

            foreach (var point in parameters.Points)
            {
                var stepPoint = new DoPointStep(string.Format("Поверка точки {0} {1}", point.Pressure, _unit.ToStr()),
                    _adts, param, point, parameters.Rate, parameters.Unit, ChConfig.EthChannel, ChConfig.UsrChannel, _logger);
                step = new CheckStepConfig(stepPoint, false, point.IsAvailable);
                AttachStep(step.Step);
                stepPoint.SetBuffer(_dataBuffer);
                steps.Add(step);
            }

            // добавление шага завешения
            step = new CheckStepConfig(new EndStep("Завершение поверки", _adts, _logger), true);
            AttachStep(step.Step);
            steps.Add(step);

            if (Steps != null) // Отвязать события прошлых шагов
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
                _result.PointsResults.Add(_resultPoint);
                _result.CheckTime = DateTime.Now;
            }
            _dataBuffer.Clear();
        }

        protected override void OnEndMethod(EventArgs e)
        {
            _dataBuffer.Clear();
            base.OnEndMethod(e);
        }
    }
}
