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
using Tools.View;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// Конфигурация самой проверки и её исполнение
    /// </summary>
    public class PressureSensorPointsConfigVm:INotifyPropertyChanged
    {
        private double _minP = 0;
        private double _maxP = 100;
        private double _minU = 2;
        private double _maxU = 5;
        private readonly IDPI620Driver _dpi620;

        private readonly Func<double, double> _getUbyPressure;
        private readonly Func<double, double> _getdUbyU;

        public PressureSensorPointsConfigVm(Func<double, double> getUbyPressure, Func<double, double> getdUbyU, IDPI620Driver dpi620)
        {
            _dpi620 = dpi620;
            Points = new ObservableCollection<PointViewModel>();
            NewConfig = new PointConfigViewModel();
            _getUbyPressure = getUbyPressure?? ((press) =>
            {
                if (press >= _maxP)
                    return _maxU;
                if (press <= _minP)
                    return _minU;
                var val = _minU + (press - _minP) * ((_maxU - _minU)/(_maxP - _minP));
                return val;
            });
            _getdUbyU = getdUbyU ?? ((u) => 0.1);
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
        }

        private void Autoread(AutoreadState arg)
        {
            while (!arg.Cancel.WaitHandle.WaitOne(arg.PeriodRepeat))
            {
                var item = new MeasuringPoint()
                {
                    TimeStamp = DateTime.Now - arg.StartTime,
                    U = _dpi620.GetValue(1, PressureUnit),
                };
                ReadedPoints.Add(item);
                _logger.Trace($"{Name} readed repeat: {item} {SelectedUnit}");
            }
            arg.AutoreadWh.Set();
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
        /// Фактическое напряжение
        /// </summary>
        public double UReal { get; set; }

        /// <summary>
        /// Фактическая погрешность
        /// </summary>
        public double dUReal { get; set; }

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
