using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using KipTM.Interfaces;
using Tools.View;
using Tools.View.ModalContent;
using Graphic;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Выпонение проверки
    /// </summary>
    public class PressureSensorRunVm : INotifyPropertyChanged, IUserVmAsk, IDisposable
    {
        private ModalState _modalState = new ModalState();
        private readonly LinesInOutViewModel _inOutLines;
        //private Action<Action> _invoker = act => act();
        private string _note;
        private double _checkProgress;
        private bool _isRun;
        private bool _isAsk;
        private PointViewModel _selectedPoint;
        //private PointConfigViewModel _newConfig;
        private readonly TimeSpan _periodViewGraphic = TimeSpan.FromSeconds(300);
        private MeasuringPoint _lastMeasuredPoint;
        private Units _pUnit;
        private Units _outUnit;
        private readonly IContext _context;


        /// <summary>
        /// Выпонение проверки
        /// </summary>
        /// <param name="unit">Единицы измерения</param>
        /// <param name="outUnit"></param>
        /// <param name="context"></param>
        public PressureSensorRunVm(Units unit, Units outUnit, IContext context)
        {
            _pUnit = unit;
            _outUnit = outUnit;
            _context = context;
            _inOutLines = new LinesInOutViewModel(
                "I", $"I, {outUnit.ToStringLocalized(CultureInfo.CurrentUICulture)}", Color.Black, 1, _periodViewGraphic,
                "P", $"P,  {unit.ToStringLocalized(CultureInfo.CurrentUICulture)}", Color.Brown, 2, _periodViewGraphic);
            Points = new ObservableCollection<PointViewModel>();
            //NewConfig = new PointConfigViewModel(_context);
        }

        /// <summary>
        /// Список выбранных точек
        /// </summary>
        public ObservableCollection<PointViewModel> Points { get; set; }

        /// <summary>
        /// Текущая выбранная точка
        /// </summary>
        public PointViewModel SelectedPoint
        {
            get { return _selectedPoint; }
            set
            {
                _selectedPoint = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        ///// <summary>
        ///// Конфигурация для новой точки
        ///// </summary>
        //public PointConfigViewModel NewConfig
        //{
        //    get { return _newConfig; }
        //    set
        //    {
        //        _newConfig = value;
        //        OnPropertyChanged();
        //        //_invoker(() => OnPropertyChanged());
        //    }
        //}

        /// <summary>
        /// Линии на графике
        /// </summary>
        public LinesInOutViewModel Lines { get { return _inOutLines; } }

        /// <summary>
        /// Текущее значение измерение
        /// </summary>
        public MeasuringPoint LastMeasuredPoint
        {
            get { return _lastMeasuredPoint; }
            set
            {
                _lastMeasuredPoint = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressureUnit
        {
            get
            {
                return _pUnit;
            }
            set
            {
                if (value == _pUnit)
                    return;
                _pUnit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения напряжения
        /// </summary>
        public Units OutUnit
        {
            get
            {
                return _outUnit;
            }
            set
            {
                if (value == _outUnit)
                    return;
                _outUnit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Прогресс выполнения проверки
        /// </summary>
        public double CheckProgress
        {
            get { return _checkProgress; }
            set
            {
                _checkProgress = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Проверка выполняется
        /// </summary>
        public bool IsRun
        {
            get { return _isRun; }
            set
            {
                _isRun = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Состояние модального окна
        /// </summary>
        public ModalState ModalState
        {
            get { return _modalState; }
            set
            {
                _modalState = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        #region IUserVmAsk

        /// <summary>
        /// Выполняется запрос с подтвержнелием
        /// </summary>
        public bool IsAsk
        {
            get { return _isAsk; }
            set
            {
                _isAsk = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Пояснение
        /// </summary>
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Установить действие на подтверждение 
        /// </summary>
        /// <param name="accept"></param>
        public void SetAcceptAction(Action accept)
        {
            _doAccept = accept;
        }

        public void ResetSetAcceptAction()
        {
            _doAccept = () => { };
        }

        public IDisposable ShowModalAsk(string title, string msg, EventWaitHandle wh)
        {
            var whInv = new ManualResetEvent(false);
            IDisposable disp = null;
            _context.Invoke(() =>
            {
                disp = ModalState.AskModal(string.IsNullOrEmpty(title) ? msg : $"{title}\n{msg}", wh);
                whInv.Set();
            });
            whInv.WaitOne();
            return disp;
        }

        public void AskModal(string title, string msg, CancellationToken cancel)
        {//TODO вынести или упростить
            ModalState.IsShowModal = true;
            var wh = new ManualResetEvent(false);
            ModalState.Ask(string.IsNullOrEmpty(title) ? msg : $"{title}\n{msg}", wh);
            //_invoker(() => ModalState.Ask(string.IsNullOrEmpty(title) ? msg : $"{title}\n{msg}", wh));
            var whs = new[] { wh, cancel.WaitHandle };
            WaitHandle.WaitAny(whs);
            ModalState.IsShowModal = false;
        }

        #endregion

        public void UpdatePoints(IEnumerable<PressureSensorPointConf> points)
        {
            _context.Invoke(() =>
            {
                Points.Clear();
                foreach (var point in points)
                {
                    var pointVm = new PointViewModel(_context) { Result = new PointResultViewModel() };
                    pointVm.UpdateConf(point);
                    Points.Add(pointVm);
                }
            });
        }

        /// <summary>
        /// Установка единицы измерения давления
        /// </summary>
        /// <param name="pressureUnit"></param>
        public void SetPerssureUnit(Units pressureUnit)
        {
            _context.Invoke(() => PressureUnit = pressureUnit);
        }

        /// <summary>
        /// Установка единицы измерения давления
        /// </summary>
        /// <param name="ourUnit"></param>
        public void SetOutUnit(Units ourUnit)
        {
            _context.Invoke(() => OutUnit = ourUnit);
        }

        public void ToBaseState()
        {
            _context.Invoke(() =>
            {
                ResetSetAcceptAction();
                Note = "";
                IsAsk = false;
            });
        }

        /// <summary>
        /// Обновить состояние визуальной модели результата
        /// </summary>
        /// <param name="checkResult"></param>
        /// <param name="isActivePresSrc"></param>
        internal void UpdateResult(PressureSensorResult checkResult, bool isActivePresSrc)
        {
            _context.Invoke(() =>DoUpdateResult(checkResult, isActivePresSrc));
        }

        private void DoUpdateResult(PressureSensorResult checkResult, bool isActivePresSrc)
        {
            foreach (var point in Points)
            {
                var res = checkResult.Points.FirstOrDefault(el => el.Index == point.Index);
                if (res == null)
                    continue;
                if (point.Result == null)
                    point.Result = new PointResultViewModel();

                if (double.IsNaN(res.Result.OutPutValueBack))
                { // прямой ход
                    if (isActivePresSrc)
                    {
                        //если источник давления управвляем - результат уже заполнен.
                        point.Result.PressureReal = res.Result.PressureValue;
                    }
                    point.Result.IReal = res.Result.OutPutValue;
                    point.Result.dIReal = res.Result.OutPutValue - res.Result.VoltagePoint;
                    point.Result.IsCorrect = res.Result.IsCorrect;
                }
                else
                { // обратный ход
                    point.Result.Iback = res.Result.OutPutValueBack;
                    point.Result.dIvar = Math.Abs(res.Result.OutPutValue - res.Result.OutPutValueBack);
                }
            }
        }

        ///// <summary>
        ///// Добавить точку проверку
        ///// </summary>
        //public ICommand AddPoint => new CommandWrapper(OnCallAddPoint);

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand StartCheck { get { return new CommandWrapper(OnCallStartCheck); } }

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand Accept { get { return new CommandWrapper(DoAccept); } }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        public ICommand PauseCheck { get { return new CommandWrapper(OnCallPauseCheck); } }

        /// <summary>
        /// Остановить проверку
        /// </summary>
        public ICommand StopCheck { get { return new CommandWrapper(OnCallStopCheck); } }

        ///// <summary>
        ///// Добавить точку проверку
        ///// </summary>
        //public event Action CallAddPoint;
        //protected virtual void OnCallAddPoint()
        //{
        //    CallAddPoint?.Invoke();
        //}

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public event Action CallStartCheck;
        protected virtual void OnCallStartCheck()
        {
            CallStartCheck?.Invoke();
        }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        public event Action CallPauseCheck;
        protected virtual void OnCallPauseCheck()
        {
            CallPauseCheck?.Invoke();
        }

        /// <summary>
        /// Остановить проверку
        /// </summary>
        public event Action CallStopCheck;
        protected virtual void OnCallStopCheck()
        {
            CallStopCheck?.Invoke();
        }

        /// <summary>
        /// Добавить очередную измеренную точку
        /// </summary>
        /// <param name="value"></param>
        internal void AddLastMeasured(MeasuringPoint value)
        {
            _context.Invoke(() =>
            {
                _inOutLines.AddPoint(value.TimeStamp, value.I, value.Pressure);
                LastMeasuredPoint = value;
            });
        }

        /// <summary>
        /// Уствновить состояние выполнения
        /// </summary>
        /// <param name="isRun"></param>
        internal void SetIsRun(bool isRun)
        {
            _context.Invoke(() => IsRun = isRun);
        }

        internal void ClearAllLines()
        {
            _context.Invoke(() => _inOutLines.CLearAllLines());
        }

        internal Action DoAccept { get { return () => { this._doAccept(); }; } }
        internal Action _doAccept = () => { };

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //TODO: вызвать зависимые методы презентора
        }

        #endregion
    }
}
