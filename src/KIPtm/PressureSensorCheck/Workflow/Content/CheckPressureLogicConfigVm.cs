using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Interfaces;
using PressureSensorCheck.Workflow.Content;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация логики проверки
    /// </summary>
    public class CheckPressureLogicConfigVm : INotifyPropertyChanged
    {
        public class ConfData
        {
            public double TolerancePercentSigma;
            public double TolerancePercentVpi;
            public double VpiMin;
            public double VpiMax;
            public Units PressureUnit;
            public OutGange OutputRange;
            
        }

        private ConfData _data;

        private readonly IContext _context;

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfigVm(IContext context)
        {
            _context = context;
            _data = new ConfData();
            Points = new ObservableCollection<PointConfigViewModel>();
        }

        /// <summary>
        /// Максимум диапазона
        /// </summary>
        public double VpiMax
        {
            get { return _data.VpiMax; }
            set
            {
                _data.VpiMax = value;
                OnSettedData(_data);
                OnPropertyChanged(nameof(VpiMax));
            }
        }

        /// <summary>
        /// Минимум диапазона
        /// </summary>
        public double VpiMin
        {
            get { return _data.VpiMin; }
            set
            {
                _data.VpiMin = value;
                OnSettedData(_data);
                OnPropertyChanged(nameof(VpiMin));
            }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public IEnumerable<Units> Units { get; set; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressUnit
        {
            get { return _data.PressureUnit; }
            set
            {
                if (value == _data.PressureUnit)
                    return;
                _data.PressureUnit = value;
                OnSettedData(_data);
                OnPropertyChanged("PressUnit");
            }
        }

        /// <summary>
        /// Допуск по приведенной погрешности
        /// </summary>
        public string TolerancePercentSigma
        {
            get { return _data.TolerancePercentSigma.ToString(); }
            set
            {
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _data.TolerancePercentSigma = dval;
                OnSettedData(_data);
                OnPropertyChanged("TolerancePercentSigma");
            }
        }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public string TolerancePercentVpi
        {
            get { return _data.TolerancePercentVpi.ToString(); }
            set
            {
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _data.TolerancePercentVpi = dval;
                OnSettedData(_data);
                OnPropertyChanged("TolerancePercentVpi");
            }
        }

        /// <summary>
        /// Варианты выходного диапазона
        /// </summary>
        public IEnumerable<OutGange> OutputRanges { get; set; }

        /// <summary>
        /// Выбранный выходной диапазон
        /// </summary>
        public OutGange OutputRange
        {
            get { return _data.OutputRange; }
            set
            {
                if (value == _data.OutputRange)
                    return;
                _data.OutputRange = value;
                OnSettedData(_data);
                OnPropertyChanged(nameof(OutputRange));
            }
        }

        /// <summary>
        /// Точки проверки
        /// </summary>
        public ObservableCollection<PointConfigViewModel> Points { get; set; }

        /// <summary>
        /// Калькулятор точки
        /// </summary>
        public CheckPointVm PointCalculator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vpiMax"></param>
        /// <param name="vpiMin"></param>
        /// <param name="units"></param>
        /// <param name="pressureUnit"></param>
        /// <param name="outputRanges"></param>
        /// <param name="range"></param>
        /// <param name="toleranceSigma"></param>
        /// <param name="toleranceVpi"></param>
        public void SetBaseStates(double vpiMax, double vpiMin, IEnumerable<Units> units, Units pressureUnit, IEnumerable<OutGange> outputRanges, OutGange range, double toleranceSigma, double toleranceVpi, double uMin, double uMax)
        {
            _data.VpiMax = vpiMax;
            _data.VpiMin = vpiMin;
            Units = units;
            _data.PressureUnit = pressureUnit;
            OutputRanges = outputRanges;
            _data.OutputRange = range;
            _data.TolerancePercentVpi = toleranceVpi;
            _data.TolerancePercentSigma = toleranceSigma;
            PointCalculator = new CheckPointVm();
            PointCalculator.TolerancePercentVpi = _data.TolerancePercentVpi;
            PointCalculator.TolerancePercentSigma = _data.TolerancePercentSigma;
            PointCalculator.Imax = uMax;
            PointCalculator.Imin = uMin;
            PointCalculator.Pmax = vpiMax;
            PointCalculator.Pmin = vpiMin;
            PointCalculator.UpdateFormulas();
        }

        public event Action<ConfData> SettedData;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        protected virtual void OnSettedData(ConfData data)
        {
            SettedData?.Invoke(data);
        }

        public PointConfigViewModel AddPoint(double pressure, double i, double di, Units unit)
        {
            var point = new PointConfigViewModel(_context, pressure, i, di, unit);
            _context.Invoke(()=>Points.Add(point));
            return point;
        }
    }
}