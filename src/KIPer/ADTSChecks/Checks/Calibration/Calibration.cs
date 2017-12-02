using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using ADTSChecks.Checks;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSChecks.Model.Steps.ADTSCalibration;
using ADTSData;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Archive;
using KipTM.Archive;
using KipTM.Model.Checks;
using Tools;

namespace ADTSChecks.Model.Checks
{
    public class Calibration : CheckBaseADTS
    {
        //public static string Key = "Калибровка ADTS";
        public const string key = "Калибровка ADTS";

        //public const string KeyPoints = "Points";
        //public const string KeyRate = "Rate";
        //public const string KeyUnit = "Unit";

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

        public Calibration(NLog.Logger logger)
            : base(logger)
        {
            base.Key = Calibration.key;
            MethodName = "Калибровка ADTS";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyes"></param>
        /// <returns></returns>
        public override object GetCustomConfig(IPropertyPool propertyes)
        {
            //var propertyes = propertyPool.ByKey(ChannelKey);
            var points = propertyes.GetProperty<List<ADTSPoint>>(CheckBaseADTS.KeyPoints);
            var channel = propertyes.GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
            var rate = propertyes.GetProperty<double>(CheckBaseADTS.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(CheckBaseADTS.KeyUnit);
            return new ADTSParameters(channel, points, rate, unit);
        }

        /// <summary>
        /// Заполнение списка шагов 
        /// </summary>
        /// <returns></returns>
        public bool FillSteps(ADTSParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));

            ChConfig.Channel = parameters.CalibChannel;

            var steps = new List<CheckStepConfig>();

            // добавление шага инициализации
            var step = new CheckStepConfig(new InitStep("Инициализация калибровки", _adts, ChConfig.Channel, _logger), true);
            steps.Add(step);
            AttachStep(step.Step);

            // добавление шагов прохождения точек
            Parameters param;
            if (ChConfig.Channel.Name == ADTSModel.Ps)
                param = Parameters.PS;
            else if (ChConfig.Channel.Name == ADTSModel.Pt)
                param = Parameters.PT;
            else param = Parameters.PS;
            foreach (var point in parameters.Points)
            {
                var stepPoint = new DoPointStep(string.Format("Калибровка точки {0} {1}", point.Pressure, parameters.Unit.ToStr()),
                    _adts, param, point, parameters.Rate, parameters.Unit, ChConfig.EthChannel, ChConfig.UsrChannel, _logger);
                step = new CheckStepConfig(stepPoint, false, point.IsAvailable);
                stepPoint.SetBuffer(_dataBuffer);
                AttachStep(step.Step);
                steps.Add(step);
            }

            // добавление шага подтверждения калибровки
            step = new CheckStepConfig(new FinishStep("Подтверждение калибровки", _adts, ChConfig.UsrChannel, _logger), true);
            AttachStep(step.Step);
            steps.Add(step);

            // добавление шага перевода в базовое состояние
            step = new CheckStepConfig(new ToBaseStep("Перевод в базовое состояние", _adts, _logger), true);
            AttachStep(step.Step);
            steps.Add(step);
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
                _result.PointsResults.Add(_resultPoint);
            _dataBuffer.Clear();
        }

        protected override void OnEndMethod(EventArgs e)
        {
            _dataBuffer.Clear();
            base.OnEndMethod(e);
        }

    }
}
