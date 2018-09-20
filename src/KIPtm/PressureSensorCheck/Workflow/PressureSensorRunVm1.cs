using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorData;
using Tools.View;
using Tools.View.ModalContent;
using Graphic;
using KipTM.EventAggregator;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Выпонение проверки
    /// </summary>
    public class PressureSensorRunVm1 : INotifyPropertyChanged, IUserVmAsk, IDisposable
    {
        private ModalState _modalState = new ModalState();
        private readonly LinesInOutViewModel _inOutLines;
        //private Action<Action> _invoker = act => act();
        private string _note;
        private double _checkProgress;
        private bool _isRun;
        private bool _isAsk;
        private PointViewModel _selectedPoint;
        private PointConfigViewModel _newConfig;
        private readonly TimeSpan _periodViewGraphic = TimeSpan.FromSeconds(300);
        private MeasuringPoint _lastMeasuredPoint;
        private Units _pUnit;
        private Units _outUnit;

        /// <summary>
        /// Выпонение проверки
        /// </summary>
        /// <param name="unit">Единицы измерения</param>
        /// <param name="outUnit"></param>
        public PressureSensorRunVm1(Units unit, Units outUnit)
        {
            _pUnit = unit;
            _outUnit = outUnit;
            _inOutLines = new LinesInOutViewModel(
                "I", $"I, {outUnit.ToStringLocalized(CultureInfo.CurrentUICulture)}", Color.Black, 1, _periodViewGraphic,
                "P", $"P,  {unit.ToStringLocalized(CultureInfo.CurrentUICulture)}", Color.Brown, 2, _periodViewGraphic);
            Points = new ObservableCollection<PointViewModel>();
            NewConfig = new PointConfigViewModel();
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

        /// <summary>
        /// Конфигурация для новой точки
        /// </summary>
        public PointConfigViewModel NewConfig
        {
            get { return _newConfig; }
            set
            {
                _newConfig = value;
                OnPropertyChanged();
                //_invoker(() => OnPropertyChanged());
            }
        }

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
            return ModalState.AskModal(string.IsNullOrEmpty(title) ? msg : $"{title}\n{msg}", wh);
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

        /// <summary>
        /// Добавить точку проверку
        /// </summary>
        public ICommand AddPoint => new CommandWrapper(OnCallAddPoint);

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

        /// <summary>
        /// Добавить точку проверку
        /// </summary>
        public event Action CallAddPoint;
        protected virtual void OnCallAddPoint()
        {
            CallAddPoint?.Invoke();
        }

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

        internal void AddToLine(TimeSpan time, double inVal, double outVal)
        {
            _inOutLines.AddPoint(time, inVal, outVal);
        }

        internal void CLearAllLines()
        {
            _inOutLines.CLearAllLines();
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
