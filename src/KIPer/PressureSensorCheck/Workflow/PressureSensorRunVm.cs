using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DPI620Genii;
using KipTM.Model.Channels;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorData;
using Tools.View;
using Tools.View.ModalContent;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Выпонение проверки
    /// </summary>
    public class PressureSensorRunVm : INotifyPropertyChanged, IObserver<MeasuringPoint>
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

        /// <summary>
        /// Выпонение проверки
        /// </summary>
        /// <param name="config">конфигурация проверки</param>
        /// <param name="dpi620">драйвер DPI620Genii</param>
        /// <param name="dpiConf">контейнер конфигурации DPI620</param>
        /// <param name="result"></param>
        public PressureSensorRunVm(PressureSensorConfig config, DPI620DriverCom dpi620, DPI620GeniiConfig dpiConf, PressureSensorResult result)
        {
            Measured = new ObservableCollection<MeasuringPoint>();
            _dpi620 = dpi620;
            _dpiConf = dpiConf;
            _logger = NLog.LogManager.GetLogger("PressureSensorPointsConfigVm");
            Points = new ObservableCollection<PointViewModel>();
            NewConfig = new PointConfigViewModel();
            _config = config;
            _autoupdater = new AutoUpdater(_logger);
            Result = result;
        }

        /// <summary>
        /// Список выбранных точек
        /// </summary>
        public ObservableCollection<PointViewModel> Points { get; set; }

        /// <summary>
        /// Текущий результат
        /// </summary>
        public PressureSensorResult Result { get; set; }

        /// <summary>
        /// Время последних результатов
        /// </summary>
        public DateTime? LastResultTime { get; set; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public string PressureUnit { get; } = "мм рт.ст.";

        /// <summary>
        /// Текущая выбранная точка
        /// </summary>
        public PointViewModel SelectedPoint { get; set; }

        /// <summary>
        /// Конфигурация для новой точки
        /// </summary>
        public PointConfigViewModel NewConfig { get; set; }

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
                    U = NewConfig.U,
                    dU = NewConfig.dU,
                    Unit = PressureUnit,
                },
                Result = new PointResultViewModel()
            });
            _config.Points.Add(new PressureSensorPoint()
            {
                PressurePoint = NewConfig.Pressure,
                VoltagePoint = NewConfig.U,
                Tollerance = NewConfig.dU,
                PressureUnit = PressureUnit,
                VoltageUnit = "мА"
            });
        }

        /// <summary>
        /// Измерения
        /// </summary>
        public ObservableCollection<MeasuringPoint> Measured { get; set; }

        /// <summary>
        /// Текущее значение измерение
        /// </summary>
        public MeasuringPoint LastMeasuredPoint { get; set; }

        /// <summary>
        /// Пояснение
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Прогресс выполнения проверки
        /// </summary>
        public double CheckProgress { get; set; }

        /// <summary>
        /// Проверка выполняется
        /// </summary>
        public bool IsRun { get; set; }

        /// <summary>
        /// Выполняется запрос с подтвержнелием
        /// </summary>
        public bool IsAsk { get; set; }

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
            IsRun = true;
            if (_startTime != null)
                return;
            // подготовка внутрених переменных к старту проверки
            _startTime = DateTime.Now;
            var cancel = _cancellation.Token;
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
                return;
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
                return;
            }

            // запуск авточтения состояния
            var tUpdate = new Task((arg) => _autoupdater.Start(_dpi620, (AutoUpdater.AutoreadState)arg, _config),
                new AutoUpdater.AutoreadState(cancel, _periodAutoread, _startTime.Value, _autorepeatWh),
                cancel, TaskCreationOptions.None);
            tUpdate.Start(TaskScheduler.Default);
            var checkLogger = NLog.LogManager.GetLogger(string.Format("Check{0}",
                _startTime.Value.ToString("yy.MM.dd_hh:mm:ss.fff")));
            
            // конфигурирование шагов проверки
            var check = new PresSensorCheck(checkLogger, new DPI620Ethalon(_dpi620, inSlotNum), new DPI620Ethalon(_dpi620, outSlotNum), Result);
            check.ChConfig.UsrChannel = new PressureSensorUserChannel(this);
            check.FillSteps(_config);

            // запуск самой проверки
            var task = new Task(() =>{TryStart(cancel, check);});
            task.Start(TaskScheduler.Default);
        }

        /// <summary>
        /// Выполнение проверки с гарантированым снятием признака запуска проверки
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="check"></param>
        private void TryStart(CancellationToken cancel, PresSensorCheck check)
        {
            try
            {
                using (_autoupdater.Subscribe(this))
                {
                    if (check.Start(cancel))
                        if (!cancel.IsCancellationRequested)
                            UpdateResult(check.Result);
                }
            }
            finally
            {
                IsRun = false;
            }
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
                point.Result.PressureReal = res.PressureValue;
                point.Result.UReal = res.VoltageValue;
                point.Result.Uback = res.VoltageValueBack;
                point.Result.dUReal = res.VoltageValue - res.VoltagePoint;
                point.Result.dUvar = Math.Abs(res.VoltageValue - res.VoltageValueBack);
                point.Result.IsCorrect = point.Result.dUReal <= point.Config.dU;
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
            IsRun = false;
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
            set { _modalState = value; }
        }

        /// <summary>
        /// Вызов модального окна
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private bool AskModal(string title, string msg, CancellationToken cancel)
        {
            ModalState.IsShowModal = true;
            var res = ModalState.AskOkCancel(string.Format("{0}\n\n{1}", title, msg));
            ModalState.IsShowModal = false;
            return res;
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
    }

    public class AutoUpdater : IObservable<MeasuringPoint>
    {

        internal class AutoreadState
        {
            public AutoreadState(CancellationToken cancel, TimeSpan periodRepeat, DateTime startTime, EventWaitHandle autoreadWh)
            {
                Cancel = cancel;
                PeriodRepeat = periodRepeat;
                StartTime = startTime;
                AutoreadWh = autoreadWh;
            }

            public CancellationToken Cancel { get; }

            public TimeSpan PeriodRepeat { get; }

            public DateTime StartTime { get; }

            public EventWaitHandle AutoreadWh { get; }
        }

        public AutoUpdater(Logger logger)
        {
            _logger = logger;
            observers = new List<IObserver<MeasuringPoint>>();
        }

        private readonly Logger _logger;
        private List<IObserver<MeasuringPoint>> observers;

        public IDisposable Subscribe(IObserver<MeasuringPoint> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        internal void Start(DPI620DriverCom dpi620, AutoreadState arg, PressureSensorConfig conf)
        {
            try
            {
                while (!arg.Cancel.WaitHandle.WaitOne(arg.PeriodRepeat))
                {
                    var u = dpi620.GetValue(1);
                    var pressure = dpi620.GetValue(2);
                    var un = GetUForPressure(conf, pressure);
                    var du = un - u;
                    var qu = u / un - 1.0;
                    var item = new MeasuringPoint()
                    {
                        TimeStamp = DateTime.Now - arg.StartTime,
                        U = u,
                        Pressure = pressure,
                        dU = du,
                        Un = GetUForPressure(conf, pressure),
                        dUn = GetdUForU(conf, un),
                        qU = qu,
                        qUn = 0,
                    };
                    Publish(item);
                    _logger.Trace($"Readed repeat: P:{item.Pressure}");
                }
            }
            finally
            {
                arg.AutoreadWh.Set();
            }
        }


        private double GetUForPressure(PressureSensorConfig conf, double pressure)
        {
            var percentVpi = (pressure - conf.VpiMin) / (conf.VpiMax - conf.VpiMin);
            if (pressure < conf.VpiMin)
                percentVpi = 0;
            double uMin = 0.0;
            double uMax = 5.0;
            if (conf.OutputRange == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }
            var val = uMin + (uMax - uMin) * percentVpi;

            return val;
        }

        /// <summary>
        /// Получить допуск для конкретной точки напряжения
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private double GetdUForU(PressureSensorConfig conf, double u)
        {
            return conf.ToleranceDelta;
        }

        private void Publish(MeasuringPoint data)
        {
            if (observers == null)
                return;
            foreach (var observer in observers)
            {
                observer.OnNext(data);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<MeasuringPoint>> _observers;
            private IObserver<MeasuringPoint> _observer;

            public Unsubscriber(List<IObserver<MeasuringPoint>> observers, IObserver<MeasuringPoint> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }

    public class MeasuringPoint : INotifyPropertyChanged
    {
        /// <summary>
        /// Текущее давление
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Текущее напряжение
        /// </summary>
        public double U { get; set; }

        /// <summary>
        /// Нормативное напряжение соответствующее заданному давлению
        /// </summary>
        public double Un { get; set; }

        /// <summary>
        /// Отклонение от нормативного напряжения на заданном давлении
        /// </summary>
        public double dU { get; set; }

        /// <summary>
        /// Допустимое отклонение от нормативного напряжения на заданном давлении
        /// </summary>
        public double dUn { get; set; }

        /// <summary>
        /// Относительное отклонение от нормативного напряжения на заданном давлении
        /// </summary>
        public double qU { get; set; }

        /// <summary>
        /// Допустимое относительное отклонение от нормативного напряжения на заданном давлении
        /// </summary>
        public double qUn { get; set; }

        /// <summary>
        /// Метка времени измерения
        /// </summary>
        public TimeSpan TimeStamp { get; set; }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Описатель одной точки проверки
    /// </summary>
    public class PointViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Номер точки
        /// </summary>
        public int Index { get; set; }

        public PointConfigViewModel Config { get; set; }

        public PointResultViewModel Result { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    /// <summary>
    /// Конфигурация точки
    /// </summary>
    public class PointConfigViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Проверяемая точка давления
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Ожидаемое значение напряжения
        /// </summary>
        public double U { get; set; }

        /// <summary>
        /// Допуск по напряжению
        /// </summary>
        public double dU { get; set; }

        /// <summary>
        /// Допуск по вариации напряжения
        /// </summary>
        public double Uvar { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    /// <summary>
    /// Результат на точке
    /// </summary>
    public class PointResultViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Фактическое давление
        /// </summary>
        public double PressureReal { get; set; }

        /// <summary>
        /// Фактическое напряжение (прямой ход)
        /// </summary>
        public double UReal { get; set; }

        /// <summary>
        /// Фактическая погрешность (прямой ход)
        /// </summary>
        public double dUReal { get; set; }

        /// <summary>
        /// Фактическое напряжение (обратный ход)
        /// </summary>
        public double Uback { get; set; }

        /// <summary>
        /// Фактическая вариация
        /// </summary>
        public double Uvar { get; set; }

        /// <summary>
        /// Фактическая погрешность вариации
        /// </summary>
        public double dUvar { get; set; }

        /// <summary>
        /// Напряжение на заданной точке в допуске
        /// </summary>
        public bool IsCorrect { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public class PressureSensorUserChannel : IUserChannel
    {
        private readonly PressureSensorRunVm _vm;

        public PressureSensorUserChannel(PressureSensorRunVm vm)
        {
            _vm = vm;
        }

        public UserQueryType QueryType { get; }
        public string Message { get { return _vm.Note; } set { _vm.Note = value; } }
        public bool AcceptValue { get; set; }
        public double RealValue { get; set; }
        public bool AgreeValue { get; set; }
        public void NeedQuery(UserQueryType queryType, EventWaitHandle wh)
        {
            _vm.IsAsk = true;
            _vm.DoAccept = () => ConfigQuery(wh);
        }

        private void ConfigQuery(EventWaitHandle wh)
        {
            wh.Set();
            _vm.DoAccept = () => { };
            Message = "";
            _vm.IsAsk = false;
        }

        public event EventHandler QueryStarted;

        public void Clear()
        {
            _vm.DoAccept = () => { };
            Message = "";
            _vm.IsAsk = false;
        }
    }
}
