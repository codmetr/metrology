using System;
using System.Threading;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSData;
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
    public class DoPointStep : TestStepWithBuffer, IStoppedOnPoint, ISettedEtalonChannel, IPausedStep, ISettedUserChannel
    {
        #region members
        public const string KeyStep = "DoPointStep";
        public const string KeyPressure = "Pressure";

        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly ADTSPoint _point;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private IEtalonChannel EtalonChannel;
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private readonly ManualResetEvent _setCurrentValueAsPoint = new ManualResetEvent(false);
        private bool _isPauseAvailable = false;
        private State _stateBeforeHold = State.Control;
        private AdtsPointResult _result;

        #endregion

        public DoPointStep(string name, ADTSModel adts,
            Parameters param, ADTSPoint point, double rate, PressureUnits unit,
            IEtalonChannel etalonChannel, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            _userChannel = userChannel;
            EtalonChannel = etalonChannel;
            _result = new AdtsPointResult();
        }

        #region ITestStep

        /// <summary>
        /// Запустить шаг
        /// </summary>
        /// <param name="cancel"></param>
        public override void Start(CancellationToken cancel)
        {
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
            if (!Wait(wh, _setCurrentValueAsPoint, cancel, out isSettedCurrent))
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
            var realValue = EtalonChannel.GetEtalonValue(point, cancel);

            _userChannel.Message = string.Format( "Установлено на точке {0} {1} полученно эталонное значение {2} {3}. Для продолжения нажмите ",
                    point.ToString("F2"), _unit.ToStr(), realValue.ToString("F2"), _unit.ToStr()) + "\"{0}\"";
            wh.Reset();
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            wh.WaitOne(TimeSpan.FromSeconds(30));

            // Расчитать погрешность и зафиксировать реультата
            bool correctPoint = Math.Abs(Math.Abs(point) - Math.Abs(realValue)) <= _point.Tolerance;
            _logger.With(l => l.Trace(string.Format("Real value {0} ({1})", realValue, correctPoint ? "correct" : "incorrect")));

            _result.Point = point;
            _result.Tolerance = _point.Tolerance;
            _result.RealValue = realValue;
            _result.Unit = _unit.ToStr();
            _result.Error = Math.Abs(point) - Math.Abs(realValue);
            _result.IsCorrect = correctPoint;

            if (cancel.IsCancellationRequested)
            {
                DoEndCancel();
                return;
            }

            if (_buffer != null)
            {
                _logger.With(l => l.Trace("save result point to buffer"));
                _buffer.Append(_result);
            }

            // Сдвинуть прогресс
            OnProgressChanged(new EventArgProgress(100, string.Format("Точка {0}: Реальное значени {1}({2})",
                    point, realValue, correctPoint ? "correct" : "incorrect")));
            DoEnd(true);
            return;
        }

        #endregion

        #region ISettedEtalonChannel
        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ehalon">эталонный канал</param>
        public void SetEtalonChannel(IEtalonChannel ehalon)
        {
            EtalonChannel = ehalon;
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

        #region Service
        /// <summary>
        /// Обертка для выполнения длительной операции
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wh"></param>
        /// <param name="whSetCurentValue"></param>
        /// <param name="waitPointPeriod"></param>
        /// <param name="cancel"></param>
        /// <param name="isSettedCurrentPoint"></param>
        /// <returns></returns>
        private bool Wait(EventWaitHandle wh, EventWaitHandle whSetCurentValue, CancellationToken cancel, out bool isSettedCurrentPoint)
        {
            isSettedCurrentPoint = false;
            var whArray = new[] { wh, whSetCurentValue, cancel.WaitHandle };
            int exitIndex = WaitHandle.WaitAny(whArray);
            //todo support pause
            if (cancel.IsCancellationRequested)
                return false;
            if (exitIndex == 1)
                isSettedCurrentPoint = true;
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
