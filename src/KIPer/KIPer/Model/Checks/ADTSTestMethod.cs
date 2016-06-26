using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using ADTSData;
using ArchiveData.DTO.Params;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks.Steps;
using KipTM.Model.Checks.Steps.ADTSTest;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSTestMethod : ADTSMethodBase
    {
        public static string Key = "Поверка ADTS";

        public const string KeyPropertyPoints = "Points";
        public const string KeyPropertyRate = "Rate";
        public const string KeyPropertyUnit = "Unit";
        public const string KeyPropertyChannel = "Channel";

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

        public ADTSTestMethod(NLog.Logger logger)
            : base(logger)
        {
            MethodName = "Поверка ADTS";
            _result = new AdtsTestResults();
            _resultPoint = new AdtsPointResult();
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(IPropertyPool propertyes)
        {
            var points = propertyes.GetProperty<List<ADTSPoint>>(KeyPropertyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(KeyPropertyChannel);
            var rate = propertyes.GetProperty<double>(KeyPropertyRate);
            var unit = propertyes.GetProperty<PressureUnits>(KeyPropertyUnit);
            return FillSteps(new ADTSMethodParameters(channel, points, rate, unit));
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

            var steps = new List<ITestStep>();

            // добавление шага инициализации
            ITestStep step = new InitStep("Инициализация поверки", _adts, _calibChan, _logger);
            AttachStep(step);
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
                step = new DoPointStep(string.Format("Поверка точки {0}", point.Pressure), _adts, param, point.Pressure, point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _logger);
                AttachStep(step);
                steps.Add(step);
            }

            // добавление шага завешения
            step = new EndStep("Завершение поверки", _adts, _logger);
            AttachStep(step);
            steps.Add(step);

            if (Steps != null) // Отвязать события прошлых шагов
                foreach (var testStep in Steps)
                {
                    if (testStep != null)
                        DetachStep(testStep);
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
