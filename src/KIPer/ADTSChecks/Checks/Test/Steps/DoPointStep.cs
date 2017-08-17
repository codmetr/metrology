using System;
using System.Threading;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ArchiveData.DTO.Params;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks.Steps;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSTest
{
    public class DoPointStep : TestStep, IStoppedOnPoint, ISettedEthalonChannel, IPausedStep, ISettedUserChannel
    {
        #region members
        public const string KeyStep = "DoPointStep";
        public const string KeyPressure = "Pressure";

        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly ADTSPoint _point;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private IEthalonChannel _ethalonChannel;
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private readonly ManualResetEvent _setCurrentValueAsPoint = new ManualResetEvent(false);
        private bool _isPauseAvailable = false;
        private State _stateBeforeHold = State.Control;

        #endregion

        public DoPointStep(string name, ADTSModel adts,
            Parameters param, ADTSPoint point, double rate, PressureUnits unit,
            IEthalonChannel ethalonChannel, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            _userChannel = userChannel;
            _ethalonChannel = ethalonChannel;
        }

        #region ITestStep

        /// <summary>
        /// Запустить шаг
        /// </summary>
        /// <param name="cancel"></param>
        public override void Start(CancellationToken cancel)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            var point = _point.Pressure;

            OnStarted();
            // Установка единиц измерений
            if (!DoOne(cancel, () => _adts.SetPressureUnit(_unit, cancel), "[ERROR] Set unit for point"))
            {
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            // Установить скорость
            if (!DoOne(cancel, () => _adts.SetRate(_param, _rate, cancel), "[ERROR] Set rate for point"))
            {
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // Установить цель для ADST
            if (!DoOne(cancel, () => _adts.SetParameter(_param, point, cancel), "[ERROR] Set point"))
            {
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Дождаться установки параметра или примененения текущей точки как целевой
            IsPauseAvailable = true;
            EventWaitHandle wh = _param == Parameters.PT ? _adts.WaitPitotSetted() : _adts.WaitPressureSetted();
            bool isSettedCurrent;
            if (!Wait(wh, _setCurrentValueAsPoint, waitPointPeriod, cancel, out isSettedCurrent))
            {
                _adts.StopWaitStatus(wh);
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (isSettedCurrent)
            {
                // Установка текущего зкачения как ключевого
                _adts.StopWaitStatus(wh);
                point = _param == Parameters.PT
                    ? _adts.Pitot.GetValueOrDefault(point)
                    : _adts.Pressure.GetValueOrDefault(point);
                _adts.SetParameter(_param, point, cancel);
            }

            IsPauseAvailable = false;

            if (cancel.IsCancellationRequested)
            {
                DoEndCancel();
                return;
            }
            // Получить эталонное значение
            var realValue = _ethalonChannel.GetEthalonValue(point, cancel);

            _userChannel.Message =
                string.Format(
                    "Установлено на точке {0} {1} полученно эталонное значение {2} {3}. Для продолжения нажмите ",
                    point.ToString("F2"), _unit.ToStr(), realValue.ToString("F2"), _unit.ToStr()) + "\"{0}\"";
            wh.Reset();
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            wh.WaitOne(TimeSpan.FromSeconds(30));

            // Расчитать погрешность и зафиксировать реультата
            bool correctPoint = Math.Abs(Math.Abs(point) - Math.Abs(realValue)) <= _point.Tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.RealValue),
                    new ParameterResult(DateTime.Now, realValue)));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.Unit),
                    new ParameterResult(DateTime.Now, _unit.ToStr())));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.Error),
                    new ParameterResult(DateTime.Now, Math.Abs(point) - Math.Abs(realValue))));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.Tolerance),
                    new ParameterResult(DateTime.Now, _point.Tolerance)));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyPressure, point, ParameterType.IsCorrect),
                    new ParameterResult(DateTime.Now, correctPoint)));
            if (cancel.IsCancellationRequested)
            {
                DoEndCancel();
                return;
            }

            // Сдвинуть прогресс
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Точка {0}: Реальное значени {1}({2})",
                    point, realValue, correctPoint ? "correct" : "incorrect")));
            DoEnd(true);
            return;
        }

        #endregion

        #region ISettedEthalonChannel
        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ehalon">эталонный канал</param>
        public void SetEthalonChannel(IEthalonChannel ehalon)
        {
            _ethalonChannel = ehalon;
        }

        #endregion

        #region ISettedUserChannel
        public void SetUserChannel(IUserChannel userChannel)
        {
            _userChannel = userChannel;
        }
        #endregion

        #region IStoppedOnPoint
        /// <summary>
        /// Установить теущее точку как ключевую
        /// </summary>
        public void SetCurrentValueAsPoint()
        {
            _setCurrentValueAsPoint.Set();
        }

        #endregion

        #region IPausedStep

        /// <summary>
        /// Указывает что вызов паузы допустим
        /// </summary>
        public event EventHandler PauseAccessibilityChanged;

        protected virtual void OnPauseAccessibilityChanged()
        {
            EventHandler handler = PauseAccessibilityChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Указывает что вызов паузы допустим
        /// </summary>
        public bool IsPauseAvailable
        {
            get { return _isPauseAvailable; }
            private set
            {
                _isPauseAvailable = value;
                OnPauseAccessibilityChanged();
            }
        }

        /// <summary>
        /// Поставить на паузу
        /// </summary>
        /// <returns></returns>
        public bool Pause(CancellationToken cancel)
        {
            if (!IsPauseAvailable)
                return false;
            _stateBeforeHold = _adts.StateADTS ?? State.Control;
            if (_stateBeforeHold == State.Hold)
                _stateBeforeHold = State.Control;
            return _adts.SetState(State.Hold, cancel);
        }

        /// <summary>
        /// Восстановить с паузы
        /// </summary>
        /// <returns></returns>
        public bool Resume(CancellationToken cancel)
        {
            if (!IsPauseAvailable)
                return false;
            return _adts.SetState(_stateBeforeHold, cancel);
        }

        #endregion

        #region For view
        /// <summary>
        /// Ключевая точка
        /// </summary>
        public double Point { get { return _point.Pressure; } }

        /// <summary>
        /// Допуск на ключевой точке
        /// </summary>
        public double Tolerance { get { return _point.Tolerance; } }
        #endregion

        #region Serveice
        /// <summary>
        /// Обертка для выполнения длительной операции
        /// </summary>
        /// <param name="whEnd"></param>
        /// <param name="cancel"></param>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool DoOne(CancellationToken cancel, Func<bool> func, string errorMessage)
        {
            if (!func())
            {
                _logger.With(l => l.Trace(errorMessage));
                OnEnd(new EventArgEnd(KeyStep, false));
                return false;
            }
            if (cancel.IsCancellationRequested)
            {
                DoEndCancel();
                return false;
            }
            return true;
        }


        private bool Wait(EventWaitHandle wh, EventWaitHandle whSetCurentValue, TimeSpan waitPointPeriod, CancellationToken cancel, out bool isSettedCurrentPoint)
        {
            isSettedCurrentPoint = false;
            var whArray = new WaitHandle[] { wh, whSetCurentValue };
            int exitIndex = WaitHandle.WaitAny(whArray, waitPointPeriod);
            while (exitIndex == WaitHandle.WaitTimeout)
            {
                //todo support pause
                if (cancel.IsCancellationRequested)
                {
                    return false;
                }
                exitIndex = WaitHandle.WaitAny(whArray, waitPointPeriod);
            }
            if (exitIndex == 1)
            {
                isSettedCurrentPoint = true;
            }
            return true;
        }

        private void DoEnd(bool result)
        {
            OnEnd(new EventArgEnd(KeyStep, result));
        }

        private void DoEndCancel()
        {
            _logger.With(l => l.Trace(string.Format("Cancel test")));
            OnEnd(new EventArgEnd(KeyStep, false));
        }
        #endregion
    }
}
