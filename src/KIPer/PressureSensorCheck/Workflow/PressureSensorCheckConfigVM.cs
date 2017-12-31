using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация проверки
    /// </summary>
    public class PressureSensorCheckConfigVm : INotifyPropertyChanged
    {
        public PressureSensorCheckConfigVm(PressureSensorConfig configData)
        {
            Data = configData;
            EthalonPressure = new EthalonDescriptor();
            EthalonVoltage = new EthalonDescriptor();
            Config = new CheckPressureSensorConfig();
            DpiConfig = new DPI620GeniiConfig();
        }

        /// <summary>
        /// Фактические данные конфигурации
        /// </summary>
        /// <remarks>
        /// Использовать на разметке экрана только в случае единственного места изменения, так как без INotifyPropertyChanged
        /// </remarks>
        public PressureSensorConfig Data { get; }

        ///// <summary>
        ///// Пользователь:
        ///// </summary>
        //public string User { get; set; }

        ///// <summary>
        ///// Номер сертификата:
        ///// </summary>
        //public string SertificateNumber { get; set; }

        ///// <summary>
        ///// Дата сертификата:
        ///// </summary>
        //public string SertificateDate { get; set; }

        ///// <summary>
        ///// Принадлежит:
        ///// </summary>
        //public string Master { get; set; }

        ///// <summary>
        ///// Наименование:
        ///// </summary>
        //public string Title { get; set; }

        ///// <summary>
        ///// Тип:
        ///// </summary>
        //public string SensorType { get; set; }

        ///// <summary>
        ///// Модификация:
        ///// </summary>
        //public string SensorModel { get; set; }

        ///// <summary>
        ///// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        ///// </summary>
        //public string RegNum { get; set; }

        ///// <summary>
        ///// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        ///// </summary>
        //public string NumberLastCheck { get; set; }

        ///// <summary>
        ///// Заводской номер (номера):
        ///// </summary>
        //public string SerialNumber { get; set; }

        ///// <summary>
        ///// Поверено:
        ///// </summary>
        ///// <remarks>
        ///// Наименование величин, диапазонов, на которых поверено средство измерений (если предусмотрено методикой поверки)
        ///// </remarks>
        //public string CheckedParameters { get; set; }

        ///// <summary>
        ///// Поверено в соответствии с:
        ///// </summary>
        ///// <remarks>
        ///// Наименование документа, на основании которого выполнена поверка
        ///// </remarks>
        //public string ChecklLawBase { get; set; }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EthalonDescriptor EthalonPressure { get; set; }

        /// <summary>
        /// Эталон напряжения
        /// </summary>
        public EthalonDescriptor EthalonVoltage { get; set; }

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureSensorConfig Config { get; set; }

        ///// <summary>
        ///// Температура
        ///// </summary>
        //public double Temperature { get; set; }

        ///// <summary>
        ///// Влажность
        ///// </summary>
        //public double Humidity { get; set; }

        ///// <summary>
        ///// Давление дня
        ///// </summary>
        //public double DayPressure { get; set; }

        ///// <summary>
        ///// Напряжение сети
        ///// </summary>
        //public double CommonVoltage { get; set; }

        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfig DpiConfig { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class DPI620GeniiConfig : INotifyPropertyChanged
    {
        public DPI620GeniiConfig()
        {
            Slot1 = new DpiSlotConfig();
            Slot2 = new DpiSlotConfig();
        }
        public IEnumerable<string> Ports { get; set; }

        public string SelectPort { get; set; }

        public DpiSlotConfig Slot1 { get; set; }

        public DpiSlotConfig Slot2 { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public class DpiSlotConfig:INotifyPropertyChanged
        {
            public IEnumerable<ChannelType> ChannelTypes { get; set; }

            public ChannelType ChannelType { get; set; }

            public double From { get; set; }

            public double To { get; set; }

            public IEnumerable<Units> UnitSet { get; set; }

            public Units SelectedUnit { get; set; }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
    }

    /// <summary>
    /// Конфигурация логики проверки
    /// </summary>
    public class CheckPressureSensorConfig : INotifyPropertyChanged
    {
        private double _vpiMax;
        private double _vpiMin;
        private double _tolerancePercentVpi;
        private double _pointsOnRange = 5;
        private OutGange _outputRange;

        public CheckPressureSensorConfig()
        {
            _vpiMax = 780;
            _vpiMin = 0;
            _tolerancePercentVpi = 0.25;
            Points = new ObservableCollection<PointConfigViewModel>();
            Units = new List<string>()
            {
                "мм рт.ст."
            };
            Unit = Units.FirstOrDefault();
            OutputRanges = new[]
            {
                OutGange.I4_20mA,
                OutGange.I0_5mA,
            };
            OutputRange = OutputRanges.FirstOrDefault();
            UpdatePoints();
        }

        /// <summary>
        /// Максимум диапазона
        /// </summary>
        public double VpiMax
        {
            get { return _vpiMax; }
            set
            {
                _vpiMax = value;
                UpdatePoints();
                OnPropertyChanged("VpiMax");
            }
        }

        /// <summary>
        /// Минимум диапазона
        /// </summary>
        public double VpiMin
        {
            get { return _vpiMin; }
            set
            {
                _vpiMin = value;
                UpdatePoints();
                OnPropertyChanged("VpiMin");
            }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public IEnumerable<string> Units { get; set; }

        /// <summary>
        /// Выбранная единица измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public double TolerancePercentVpi
        {
            get { return _tolerancePercentVpi; }
            set
            {
                _tolerancePercentVpi = value;
                UpdatePoints();
                OnPropertyChanged("TolerancePercentVpi");
            }
        }

        /// <summary>
        /// Абсолютный допуск
        /// </summary>
        public double ToleranceDelta { get; set; }

        /// <summary>
        /// относительная погрешность
        /// </summary>
        public double TolerancePercentSigma { get; set; }

        /// <summary>
        /// Варианты выходного диапазона
        /// </summary>
        public IEnumerable<OutGange> OutputRanges { get; set; }

        /// <summary>
        /// Выбранный выходной диапазон
        /// </summary>
        public OutGange OutputRange
        {
            get { return _outputRange; }
            set
            {
                if (value == _outputRange)
                    return;
                _outputRange = value;
                UpdatePoints();
                OnPropertyChanged("OutputRange");
            }
        }

        /// <summary>
        /// Точки проверки
        /// </summary>
        public ObservableCollection<PointConfigViewModel> Points { get; set; }

        /// <summary>
        /// Перерассчитать точки
        /// </summary>
        private void UpdatePoints()
        {
            var max = _vpiMax;
            var min = _vpiMin;
            var tollerance = _tolerancePercentVpi;
            if (tollerance <= 0)
                return;
            if (min >= max)
                return;

            var step = (max - min) / (_pointsOnRange - 1);
            var du = (max - min) * tollerance / 100.0;
            double uMin = 0.0;
            double uMax = 5.0;
            if (OutputRange == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }
            double uStep = (uMax - uMin) / (_pointsOnRange - 1);

            Points.Clear();
            for (double i = 0; i < _pointsOnRange; i++)
            {
                var point = min + (i * step);
                var uPoint = uMin + (i * uStep);
                Points.Add(new PointConfigViewModel()
                { Pressire = point, U = uPoint, Unit = Unit, dU = tollerance });
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Описатель эталона
    /// </summary>
    public class EthalonDescriptor : INotifyPropertyChanged
    {

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Регистрационный номер:
        /// </summary>
        public string RegNum { get; set; }

        /// <summary>
        /// Разряд:
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Класс или погрешность:
        /// </summary>
        public string ErrorClass { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
