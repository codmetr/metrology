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
using KipTM.Model.Checks.Steps.ADTSCalibration;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSCheckMethod : ADTSMethodBase
    {
        public static string Key = "Калибровка ADTS";

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        public const string KeyChannel = "Channel";

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

        public ADTSCheckMethod(NLog.Logger logger) : base(logger)
        {
            MethodName = "Калибровка ADTS";
            _result = new AdtsTestResults();
            _resultPoint = new AdtsPointResult();
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(IPropertyPool propertyes)
        {
            var points = propertyes.GetProperty<List<ADTSPoint>>(ADTSCheckMethod.KeyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(ADTSCheckMethod.KeyChannel);
            var rate = propertyes.GetProperty<double>(ADTSCheckMethod.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(ADTSCheckMethod.KeyUnit);
            return FillSteps(new ADTSMethodParameters(channel, points, rate, unit));
        }

        /// <summary>
        /// Заполнение списка шагов 
        /// </summary>
        /// <returns></returns>
        public bool FillSteps(ADTSMethodParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));

            _calibChan = parameters.CalibChannel;

            //if (_userChannel == null)
            //    throw new NullReferenceException("\"UserChannel\" not fount in parameters as IUserChannel");

            var steps = new List<ITestStep>();

            // добавление шага инициализации
            ITestStep step = new InitStep("Инициализация калибровки", _adts, _calibChan, _logger);
            steps.Add(step);
            AttachStep(step);

            // добавление шага прохождения точек
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            foreach (var point in parameters.Points)
            {
                step = new DoPointStep(string.Format("Калибровка точки {0}", point.Pressure), _adts, param, point.Pressure, point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _logger);
                AttachStep(step);
                steps.Add(step);
            }

            // добавление шага подтверждения калибровки
            step = new FinishStep("Подтверждение калибровки", _adts, _userChannel, _logger);
            AttachStep(step);
            steps.Add(step);

            // добавление шага перевода в базовое состояние
            step = new ToBaseStep("Перевод в базовое состояние", _adts, _logger);
            AttachStep(step);
            steps.Add(step);
            if (Steps != null)
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
            if (_resultPoint != null)
            {
                _result.PointsResults.Add(_resultPoint);
                _resultPoint = null;
            }
        }

        #region Fill results
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
