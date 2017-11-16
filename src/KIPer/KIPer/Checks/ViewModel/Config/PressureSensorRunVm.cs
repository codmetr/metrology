using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DPI620Genii;
using NLog;
using PressureSensorCheck.Devices;
using Tools.View;
using System.Diagnostics;
using PressureSensorCheck.Data;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// Выпонение проверки
    /// </summary>
    public class PressureSensorRunVm : INotifyPropertyChanged
    {
        private double _minP = 0;
        private double _maxP = 100;
        private double _minU = 2;
        private double _maxU = 5;
        private readonly DPI620DriverCom _dpi620;
        private DPI620GeniiConfig _dpiConf;
        private readonly Logger _logger;

        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        private DateTime? _startTime = null;

        private readonly ManualResetEvent _autorepeatWh = new ManualResetEvent(true);
        private readonly TimeSpan _periodAutoread = TimeSpan.FromMilliseconds(100);

        private readonly CheckPressureSensorConfig _config;

        /// <summary>
        /// Выпонение проверки
        /// </summary>
        /// <param name="config">конфигурация проверки</param>
        /// <param name="dpi620">драйвер DPI620Genii</param>
        /// <param name="dpiConf">контейнер конфигурации DPI620</param>
        public PressureSensorRunVm(CheckPressureSensorConfig config, DPI620DriverCom dpi620, DPI620GeniiConfig dpiConf)
        {
            Measured = new ObservableCollection<MeasuringPoint>();
            _dpi620 = dpi620;
            _dpiConf = dpiConf;
            _logger = NLog.LogManager.GetLogger("PressureSensorPointsConfigVm");
            Points = new ObservableCollection<PointViewModel>();
            NewConfig = new PointConfigViewModel();
            _config = config;
        }

        /// <summary>
        /// Список выбранных точек
        /// </summary>
        public ObservableCollection<PointViewModel> Points { get; set; }

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
                    Pressire = NewConfig.Pressire,
                    U = NewConfig.U,
                    dU = NewConfig.dU,
                    Unit = PressureUnit,
                },
                Result = new PointResultViewModel()
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
        /// Прогресс выполнения проверки
        /// </summary>
        public double CheckProgress { get; set; }

        /// <summary>
        /// Проверка выполняется
        /// </summary>
        public bool IsRun { get; set; }

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand StartCheck { get { return new CommandWrapper(DoStart); } }

        private void DoStart()
        {
            IsRun = true;
            if (_startTime != null)
                return;
            _startTime = DateTime.Now;
            var cancel = _cancellation.Token;
            _autorepeatWh.Reset();
            Measured.Clear();
            try
            {
                _dpi620.SetPort(_dpiConf.SelectPort);
                _dpi620.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ShowMessage("Не удалось связаться с DPI620Genii. Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите порт подключения");
                return;
            }

            var tUpdate = new Task((arg) => Autoread((AutoreadState)arg),
                new AutoreadState(cancel, _periodAutoread, _startTime.Value, _autorepeatWh),
                cancel, TaskCreationOptions.None);
            tUpdate.Start(TaskScheduler.Default);
            var checkLogger = NLog.LogManager.GetLogger(string.Format("Check{0}",
                _startTime.Value.ToString("yy.MM.dd_hh:mm:ss.fff")));


            DPI620GeniiConfig.DpiSlotConfig inSlot;
            DPI620GeniiConfig.DpiSlotConfig outSlot;
            int inSlotNum;
            int outSlotNum;
            if (_dpiConf.Slot1.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot2;
                outSlot = _dpiConf.Slot1;
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
                ShowMessage("Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите какие датчики в каких слотах");
                return;
            }

            var check = new PressureSensorCheck.Check.PressureSensorCheck(checkLogger, new DPI620Ethalon(_dpi620, inSlotNum), new DPI620Ethalon(_dpi620, outSlotNum));
            check.FillSteps(new PressureSensorConfig()
            {
                Points = _config.Points.Select(el=>new PressureSensorPoint()
                {
                    PressureUnit = inSlot.SelectedUnit.ToString(), //TODO преобразовать в нормальное название единиц измерения
                    PressurePoint = el.U,
                    VoltagePoint = el.Pressire,
                    VoltageUnit = outSlot.SelectedUnit.ToString(), //TODO преобразовать в нормальное название единиц измерения
                    Tollerance = el.dU,
                }).ToList()
            });
            var task = new Task(() => check.Start(cancel));
            task.Start(TaskScheduler.Default);
        }

        /// <summary>
        /// Показать сообщение пользователю
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        public ICommand PauseCheck { get { return new CommandWrapper(DoPause); } }

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
        /// Авточтение
        /// </summary>
        /// <param name="arg"></param>
        private void Autoread(AutoreadState arg)
        {
            while (!arg.Cancel.WaitHandle.WaitOne(arg.PeriodRepeat))
            {
                var u = _dpi620.GetValue(1);
                var pressure = _dpi620.GetValue(2);
                var un = GetUForPressure(pressure);
                var du = un - u;
                var qu = u / un - 1.0;
                var item = new MeasuringPoint()
                {
                    TimeStamp = DateTime.Now - arg.StartTime,
                    U = u,
                    Pressure = pressure,
                    dU = du,
                    Un = GetUForPressure(pressure),
                    dUn = GetdUForU(un),
                    qU = qu,
                    qUn = 0,
                };
                Measured.Add(item);
                _logger.Trace($"Readed repeat: P:{item.Pressure} {PressureUnit}");
            }
            arg.AutoreadWh.Set();
        }

        private double GetUForPressure(double pressure)
        {
            var percentVpi = (pressure - _config.VpiMin) / (_config.VpiMax - _config.VpiMin);
            if (pressure < _config.VpiMin)
                percentVpi = 0;
            double uMin = 0.0;
            double uMax = 5.0;
            if (_config.OutputRange == OutGange.I4_20mA)
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
        private double GetdUForU(double u)
        {
            return _config.ToleranceDelta;
        }

        protected class AutoreadState
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
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
        public double Pressire { get; set; }

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
}
