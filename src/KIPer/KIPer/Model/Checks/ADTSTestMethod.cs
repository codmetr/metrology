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

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        public const string KeyChannel = "Channel";

        private AdtsTestResults _result;
        private AdtsPointResult _resultPoint;

        public ADTSTestMethod(NLog.Logger logger) : base(logger)
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
            var points = propertyes.GetProperty<List<ADTSPoint>>(ADTSTestMethod.KeyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(ADTSTestMethod.KeyChannel);
            var rate = propertyes.GetProperty<double>(ADTSTestMethod.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(ADTSTestMethod.KeyUnit);
            return Init(new ADTSMethodParameters(channel, points, rate, unit));
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public override bool Init(ADTSMethodParameters parameters)
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

            // добавление шага прохождения точек
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
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
            if (_resultPoint != null)
            {
                _result.PointsResults.Add(_resultPoint);
                _resultPoint = null;
            }
        }

        protected override void StepResultUpdated(object sender, EventArgTestResult e)
        {
            FillResult(e);
            base.StepResultUpdated(sender, e);
        }


        #region Fill results
        /// <summary>
        /// Заполнение полученных результатов проверки
        /// </summary>
        /// <param name="e"></param>
        private void FillResult(EventArgTestResult e)
        {
            foreach (var parameterResult in e.Result)
            {
                SwitchParameter(parameterResult.Key, parameterResult.Value);
            }
        }

        /// <summary>
        /// Распределить результат в нужное поле результата
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="result"></param>
        private void SwitchParameter(ParameterDescriptor descriptor, ParameterResult result)
        {
            switch (descriptor.Name)
            {
                case InitStep.KeyCalibDate:
                    _result.CheckTime = (DateTime)result.Value;
                    break;
                case DoPointStep.KeyPressure:
                    if (_resultPoint == null)
                        _resultPoint = new AdtsPointResult();
                    _resultPoint = SetProperty(_resultPoint, descriptor, result.Value);
                    break;
                default:
                    throw new KeyNotFoundException(string.Format("Received not exected key [{0}]", descriptor.Name));
            }
        }

        /// <summary>
        /// Заполнить поле по заданному типу в параметре
        /// </summary>
        /// <param name="field">Заполняемое поле</param>
        /// <param name="ptype">Тип заполняемого поля</param>
        /// <param name="value">Значние поля</param>
        /// <returns></returns>
        public AdtsPointResult SetProperty(AdtsPointResult field, ParameterDescriptor ptype, object value)
        {
            field.Point = (double)ptype.Point;

            if (ptype.PType == ParameterType.RealValue)
                field.RealValue = (double)value;
            else if (ptype.PType == ParameterType.Error)
                field.Error = (double)value;
            else if (ptype.PType == ParameterType.Tolerance)
                field.Tolerance = (double)value;
            else if (ptype.PType == ParameterType.IsCorrect)
                field.IsCorrect = (bool)value;

            return field;
        }
        #endregion

    }
}
