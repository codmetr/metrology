using System;
using System.Collections.Generic;
using ADTS;
using ADTSChecks.Model.Steps.ADTSTest;
using ADTSData;
using ArchiveData.DTO.Params;
using KipTM.Archive;
using KipTM.Model.Checks;
using Tools;

namespace ADTSChecks.Model.Checks
{
    public class ADTSTestMethod : ADTSMethodBase
    {
        public const string Key = "Поверка ADTS";

        public const string KeyPropertyPoints = "Points";
        public const string KeyPropertyRate = "Rate";
        public const string KeyPropertyUnit = "Unit";
        public const string KeyPropertyChannel = "Channel";

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

        public ADTSTestMethod(NLog.Logger logger)
            : base(logger)
        {
            base.Key = ADTSTestMethod.Key;
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
            return FillSteps(customConf as ADTSMethodParameters);
        }

        public override object GetCustomConfig(IPropertyPool propertyes)
        {
            //var propertyes = propertyPool.ByKey(ChannelKey);
            var points = propertyes.GetProperty<List<ADTSPoint>>(KeyPropertyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(KeyPropertyChannel);
            var rate = propertyes.GetProperty<double>(KeyPropertyRate);
            var unit = propertyes.GetProperty<PressureUnits>(KeyPropertyUnit);
            return new ADTSMethodParameters(channel, points, rate, unit);
        }

        /// <summary>
        /// Заполнение списка шагов 
        /// </summary>
        /// <returns></returns>
        public bool FillSteps(ADTSMethodParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSTestMethodic"));

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
            if (_calibChan == CalibChannel.PS)
                param = Parameters.PS;
            else if (_calibChan == CalibChannel.PT)
                param = Parameters.PT;
            else param = Parameters.PS;

            foreach (var point in parameters.Points)
            {
                step = new CheckStepConfig( new DoPointStep(string.Format("Поверка точки {0}", point.Pressure), _adts, param, point.Pressure,
                        point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _userChannel, _logger), false, point.IsAvailable);
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
