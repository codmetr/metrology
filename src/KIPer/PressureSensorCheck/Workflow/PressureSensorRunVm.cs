using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorData;
using Tools.View;
using Tools.View.ModalContent;
using Graphic;
using KipTM.EventAggregator;
using PressureSensorCheck.Report;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Выпонение проверки
    /// </summary>
    public class PressureSensorRunVm : INotifyPropertyChanged, IObserver<MeasuringPoint>, ISubscriber<Action<Action>>, IDisposable
    {
        //private double _minP = 0;
        //private double _maxP = 100;
        //private double _minU = 2;
        //private double _maxU = 5;
        private readonly DPI620DriverCom _dpi620;
        private DPI620GeniiConfig _dpiConf;
        private readonly Logger _logger;

        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        private DateTime? _startTime = null;

        private readonly ManualResetEvent _autorepeatWh = new ManualResetEvent(true);
        private readonly TimeSpan _periodAutoread = TimeSpan.FromMilliseconds(100);

        private readonly PressureSensorConfig _config;

        private ModalState _modalState = new ModalState();
        private AutoUpdater _autoupdater;
        private List<LineDescriptor> _lines;
        private ObservableCollection<PointData> _lineIn = new ObservableCollection<PointData>();
        private ObservableCollection<PointData> _lineOut = new ObservableCollection<PointData>();
        private TimeSpan _periodViewGraphic = TimeSpan.FromSeconds(300);
        private Action<Action> _invoker = act => act();
        private string _note;
        private double _checkProgress;
        private bool _isRun;
        private bool _isAsk;
        private PressureSensorResult _result;
        private DateTime? _lastResultTime;
        private PointViewModel _selectedPoint;
        private PointConfigViewModel _newConfig;
        private MeasuringPoint _lastMeasuredPoint;
        private readonly IEventAggregator _agregator;

        /// <summary>
        /// Выпонение проверки
        /// </summary>
        /// <param name="config">конфигурация проверки</param>
        /// <param name="dpi620">драйвер DPI620Genii</param>
        /// <param name="dpiConf">контейнер конфигурации DPI620</param>
        /// <param name="result"></param>
        public PressureSensorRunVm(PressureSensorConfig config, DPI620DriverCom dpi620, DPI620GeniiConfig dpiConf, PressureSensorResult result, IEventAggregator agregator)
        {
            Measured = new ObservableCollection<MeasuringPoint>();
            _lines = new List<LineDescriptor>() {new LineDescriptor()
                {
                    Title = "I",
                    AzixTitle = "I, A",
                    LineColor = Color.Black,
                    LimitForLine = _periodViewGraphic,
                    Source = _lineIn,
                    Width = 1,
                },
                new LineDescriptor()
                {
                    Title = "P",
                    AzixTitle = "P, mBar",
                    LineColor = Color.Brown,
                    LimitForLine = _periodViewGraphic,
                    Source = _lineOut,
                    Width = 2,
                },
            };
            _dpi620 = dpi620;
            _dpiConf = dpiConf;
            _logger = NLog.LogManager.GetLogger("PressureSensorPointsConfigVm");
            Points = new ObservableCollection<PointViewModel>();
            NewConfig = new PointConfigViewModel();
            _config = config;
            _autoupdater = new AutoUpdater(_logger);
            Result = result;
            _agregator = agregator;
            LineCleaner = new CleanerAct();
        }

        /// <summary>
        /// Список выбранных точек
        /// </summary>
        public ObservableCollection<PointViewModel> Points { get; set; }

        /// <summary>
        /// Текущий результат
        /// </summary>
        public PressureSensorResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Время последних результатов
        /// </summary>
        public DateTime? LastResultTime
        {
            get { return _lastResultTime; }
            set { _lastResultTime = value;
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Текущая выбранная точка
        /// </summary>
        public PointViewModel SelectedPoint
        {
            get { return _selectedPoint; }
            set { _selectedPoint = value;
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Конфигурация для новой точки
        /// </summary>
        public PointConfigViewModel NewConfig
        {
            get { return _newConfig; }
            set { _newConfig = value;
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Добавить точку проверку
        /// </summary>
        public ICommand AddPoint => new CommandWrapper(DoAddPoint);

        private void DoAddPoint()
        {
            Points.Add(new PointViewModel()
            {
                Config = new PointConfigViewModel()
                {
                    Pressure = NewConfig.Pressure,
                    I = NewConfig.I,
                    dI = NewConfig.dI,
                    Unit = PressureUnit,
                },
                Result = new PointResultViewModel()
            });
            _config.Points.Add(new PressureSensorPoint()
            {
                PressurePoint = NewConfig.Pressure,
                OutPoint = NewConfig.I,
                Tollerance = NewConfig.dI,
                PressureUnit = PressureUnit,
                OutUnit = Units.mA
            });
        }

        /// <summary>
        /// Измерения
        /// </summary>
        public ObservableCollection<MeasuringPoint> Measured { get; set; }

        public IEnumerable<LineDescriptor> Lines { get { return _lines; } }

        public CleanerAct LineCleaner { get; private set; }

        /// <summary>
        /// Текущее значение измерение
        /// </summary>
        public MeasuringPoint LastMeasuredPoint
        {
            get { return _lastMeasuredPoint; }
            set { _lastMeasuredPoint = value;
                OnPropertyChanged("LastMeasuredPoint");
            }
        }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressureUnit
        {
            get
            {
                return _config.Unit;
                //if (_dpiConf.Slot1.ChannelType == ChannelType.Pressure)
                //    return _dpiConf.Slot1.SelectedUnit;
                //return _dpiConf.Slot2.SelectedUnit;
            }
        }

        /// <summary>
        /// Единицы измерения напряжения
        /// </summary>
        public Units OutUnit
        {
            get
            {
                return Units.mA;
                //if (_dpiConf.Slot1.ChannelType != ChannelType.Pressure)
                //    return _dpiConf.Slot1.SelectedUnit;
                //return _dpiConf.Slot2.SelectedUnit;
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
                _invoker(() => OnPropertyChanged());
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
                _invoker(() => OnPropertyChanged());
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
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Выполняется запрос с подтвержнелием
        /// </summary>
        public bool IsAsk
        {
            get { return _isAsk; }
            set
            {
                _isAsk = value;
                _invoker(() => OnPropertyChanged());
            }
        }

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand Accept { get { return new CommandWrapper(DoAccept); } }

        internal Action DoAccept { get; set; } = () => { };

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand StartCheck { get { return new CommandWrapper(DoStart); } }

        private void DoStart()
        {
            if (_startTime != null)
                return;
            IsRun = true;
            // подготовка внутрених переменных к старту проверки
            _startTime = DateTime.Now;
            var cancel = _cancellation.Token;
            // запуск самой проверки
            var task = new Task(() =>
            {
                try
                {
                    var check = Config(cancel);
                    if (check == null)
                        return;
                    if (cancel.IsCancellationRequested)
                        return;
                    TryStart(check, cancel);
                }
                finally
                {
                    IsRun = false;
                    _startTime = null;
                }
            });
            task.Start(TaskScheduler.Default);
        }

        /// <summary>
        /// Конфигурация оборудования
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private PresSensorCheck Config(CancellationToken cancel)
        {
            _autorepeatWh.Reset();
            Measured.Clear();

            // выбор входных и выходных слотов
            DPI620GeniiConfig.DpiSlotConfig inSlot;
            DPI620GeniiConfig.DpiSlotConfig outSlot;
            int inSlotNum;
            int outSlotNum;
            if (_dpiConf.Slot1.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot1;
                outSlot = _dpiConf.Slot2;
                inSlotNum = 1;
                outSlotNum = 2;
            }
            else if (_dpiConf.Slot2.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot2;
                outSlot = _dpiConf.Slot1;
                inSlotNum = 2;
                outSlotNum = 1;
            }
            else
            {
                ShowMessage("Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите какие датчики в каких слотах", cancel);
                return null;
            }

            // попытка подключения к DPI 620
            try
            {
                _dpi620.SetPort(_dpiConf.SelectPort);
                _dpi620.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ShowMessage("Не удалось связаться с DPI620Genii. Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите порт подключения", cancel);
                return null;
            }

            // запуск авточтения состояния
            var tUpdate = new Task((arg) => _autoupdater.Start(_dpi620, (AutoUpdater.AutoreadState)arg, _config),
                new AutoUpdater.AutoreadState(cancel, _periodAutoread, _startTime.Value, _autorepeatWh),
                cancel, TaskCreationOptions.None);
            tUpdate.Start(TaskScheduler.Default);
            var checkLogger = NLog.LogManager.GetLogger(string.Format("Check{0}",
                _startTime.Value.ToString("yy.MM.dd_hh:mm:ss.fff")));

            // конфигурирование шагов проверки
            var check = new PresSensorCheck(checkLogger,
                new DPI620Ethalon(_dpi620, inSlotNum, ChannelType.Pressure, inSlot.SelectedUnit, _config.Unit),
                new DPI620Ethalon(_dpi620, outSlotNum, ChannelType.Current, outSlot.SelectedUnit, Units.mA), Result);
            check.ChConfig.UsrChannel = new PressureSensorUserChannel(this);
            check.FillSteps(_config);
            _lineIn.Clear();
            _lineOut.Clear();
            return check;
        }

        /// <summary>
        /// Выполнение проверки с гарантированым снятием признака запуска проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="cancel"></param>
        private void TryStart(PresSensorCheck check, CancellationToken cancel)
        {
            try
            {
                using (_autoupdater.Subscribe(this))
                {
                    check.ResultUpdated +=CheckOnResultUpdated;
                    if (check.Start(cancel))
                        if (!cancel.IsCancellationRequested)
                            UpdateResult(check.Result);
                }
            }
            finally
            {
                check.ResultUpdated -= CheckOnResultUpdated;
            }
        }

        private void CheckOnResultUpdated(object sender, EventArgs eventArgs)
        {
            var check = sender as PresSensorCheck;
            if(check!=null)
                UpdateResult(check.Result);
        }

        /// <summary>
        /// Обновить состояние визуальной модели результата
        /// </summary>
        /// <param name="checkResult"></param>
        private void UpdateResult(PressureSensorResult checkResult)
        {
            Result = checkResult;
            foreach (var point in Points)
            {
                var res = checkResult.Points.FirstOrDefault(el => Math.Abs(el.PressurePoint - point.Config.Pressure) < double.Epsilon);
                if(res == null)
                    continue;
                if(point.Result == null)
                    point.Result = new PointResultViewModel();
                point.Result.PressureReal = res.PressureValue;
                point.Result.IReal = res.VoltageValue;
                point.Result.Iback = res.VoltageValueBack;
                point.Result.dIReal = res.VoltageValue - res.VoltagePoint;
                point.Result.dIvar = Math.Abs(res.VoltageValue - res.VoltageValueBack);
                point.Result.IsCorrect = point.Result.dIReal <= point.Config.dI;
            }
            LastResultTime = DateTime.Now;
        }

        /// <summary>
        /// Показать сообщение пользователю
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cancel"></param>
        private void ShowMessage(string msg, CancellationToken cancel)
        {
            AskModal("", msg, cancel);
        }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        public ICommand PauseCheck { get { return new CommandWrapper(DoPause); } }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        private void DoPause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Остановить проверку
        /// </summary>
        public ICommand StopCheck { get { return new CommandWrapper(DoStop); } }

        private void DoStop()
        {
            IsRun = false;
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
            _autorepeatWh.WaitOne();
            _dpi620.Close();
            _startTime = null;
        }

        /// <summary>
        /// Состояние модального окна
        /// </summary>
        public ModalState ModalState
        {
            get { return _modalState; }
            set { _modalState = value;
                _invoker(() => OnPropertyChanged()); }
        }

        /// <summary>
        /// Вызов модального окна
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private void AskModal(string title, string msg, CancellationToken cancel)
        {
            ModalState.IsShowModal = true;
            var wh = new ManualResetEvent(false);
            _invoker(()=>ModalState.Ask(string.IsNullOrEmpty(title)? msg: $"{title}\n{msg}", wh));
            var whs = new[] {wh, cancel.WaitHandle};
            WaitHandle.WaitAny(whs);
            ModalState.IsShowModal = false;
        }
        public void OnEvent(Action<Action> message)
        {
            _invoker = message;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IObserver<MeasuringPoint>

        public void OnNext(MeasuringPoint value)
        {
            Measured.Add(value);
            _lineIn.Add(new PointData()
            {
                Time = value.TimeStamp,
                Value = value.I
            });
            _lineOut.Add(new PointData()
            {
                Time = value.TimeStamp,
                Value = value.Pressure
            });
            LastMeasuredPoint = value;
            _logger.Trace($"Readed repeat: P:{value.Pressure} {PressureUnit}");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            if (IsRun)
            {
                _cancellation.Cancel();
                _autorepeatWh.WaitOne(TimeSpan.FromSeconds(10));
                _dpi620.Close();
            }
        }
    }
}
