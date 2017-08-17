using System;
using System.Collections.Generic;
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
    public class Calibration : CheckBase
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
            var points = propertyes.GetProperty<List<ADTSPoint>>(CheckBase.KeyPoints);
            var channel = propertyes.GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
            var rate = propertyes.GetProperty<double>(CheckBase.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(CheckBase.KeyUnit);
            return new ADTSParameters(channel, points, rate, unit);
        }

        /// <summary>
        /// Заполнение списка шагов 
        /// </summary>
        /// <returns></returns>
        public bool FillSteps(ADTSParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));

            _calibChan = parameters.CalibChannel;

            var steps = new List<CheckStepConfig>();

            // добавление шага инициализации
            var step = new CheckStepConfig(new InitStep("Инициализация калибровки", _adts, _calibChan, _logger), true);
            steps.Add(step);
            AttachStep(step.Step);

            // добавление шагов прохождения точек
            Parameters param;
            if (_calibChan.Name == ADTSModel.Ps)
                param = Parameters.PS;
            else if (_calibChan.Name == ADTSModel.Pt)
                param = Parameters.PT;
            else param = Parameters.PS;
            foreach (var point in parameters.Points)
            {
                step = new CheckStepConfig( new DoPointStep(string.Format("Калибровка точки {0} {1}", point.Pressure, parameters.Unit.ToStr()),
                    _adts, param, point, parameters.Rate, parameters.Unit, _ethalonChannel,
                    _userChannel, _logger), false, point.IsAvailable);

                AttachStep(step.Step);
                steps.Add(step);
            }

            // добавление шага подтверждения калибровки
            step = new CheckStepConfig(new FinishStep("Подтверждение калибровки", _adts, _userChannel, _logger), true);
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

        protected override void StepEnd(object sender, EventArgEnd e)
        {
            if (e.Key == DoPointStep.KeyStep && _resultPoint != null)
            {
                _result.PointsResults.Add(_resultPoint);
                OnResultUpdated(new EventArgTestStepResult(e.Key, _resultPoint));
                _resultPoint = null;
            }
            else if (e.Key == InitStep.KeyStep)
            {
                OnResultUpdated(new EventArgTestStepResult(e.Key, _result.CheckTime));
            }
        }

        #region Fill results
        /// <summary>
        /// Распределить результат в нужное поле результата
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="result"></param>
        protected override void SwitchParameter(ParameterDescriptor descriptor, ParameterResult result)
        {
            switch (descriptor.Name)
            {
                case InitStep.KeyCalibDate:
                    _result.CheckTime = (DateTime)result.Value;
                    break;
                case DoPointStep.KeyPressure:
                    if (_resultPoint == null)
                        _resultPoint = new AdtsPointResult();
                    _resultPoint.SetProperty(descriptor, result.Value);
                    break;
                default:
                    throw new KeyNotFoundException(string.Format("Received not exected key [{0}]", descriptor.Name));
            }
        }
        #endregion

    }
}
