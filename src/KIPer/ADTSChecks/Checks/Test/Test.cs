using System;
using System.Collections.Generic;
using ADTS;
using ADTSChecks.Checks;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSChecks.Model.Steps.ADTSTest;
using ADTSData;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Archive;
using KipTM.Archive;
using KipTM.Model.Checks;
using Tools;

namespace ADTSChecks.Model.Checks
{
    public class Test : CheckBase
    {
        public const string key = KeysDic.Test;

        //public const string KeyPropertyPoints = "Points";
        //public const string KeyPropertyRate = "Rate";
        //public const string KeyPropertyUnit = "Unit";
        //public const string KeyPropertyChannel = "Channel";
        public PressureUnits _unit;

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

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
            _calibChan = parameters.CalibChannel;

            //if (_userChannel == null)
            //    throw new NullReferenceException("\"UserChannel\" not fount in parameters as IUserChannel");

            var steps = new List<CheckStepConfig>();

            // добавление шага инициализации
            CheckStepConfig step = new CheckStepConfig(new InitStep("Инициализация поверки", _adts, _calibChan, _logger), true);
            AttachStep(step.Step);
            steps.Add(step);

            // добавление шагов прохождения точек
            Parameters param;
            if (_calibChan.Name == ADTSModel.Ps)
                param = Parameters.PS;
            else if (_calibChan.Name == ADTSModel.Pt)
                param = Parameters.PT;
            else param = Parameters.PS;

            foreach (var point in parameters.Points)
            {
                step = new CheckStepConfig(new DoPointStep(string.Format("Поверка точки {0} {1}", point.Pressure, _unit.ToStr()), _adts, param, point,
                    parameters.Rate, parameters.Unit, _ethalonChannel, _userChannel, _logger), false, point.IsAvailable);
                AttachStep(step.Step);
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
                //OnResultUpdated(new EventArgTestStepResult(e.Key, _result.CheckTime));
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
