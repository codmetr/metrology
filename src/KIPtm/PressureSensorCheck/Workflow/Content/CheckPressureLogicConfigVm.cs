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
        private readonly IContext _context;
        private string _tolerancePercentSigmaStr;
        private double _tolerancePercentSigma;
        private string _tolerancePercentVpiStr;
        private double _tolerancePercentVpi;
        private double _pointsOnRange = 5;

        private string _vpiMinStr;
        private string _vpiMaxStr;
        private double _vpiMin;
        private double _vpiMax;
        private Units _pressureUnit;
        private OutGange _outputRange;

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfigVm(IContext context, PressureSensorConfig data)
        {
            _context = context;
            Data = data;
            Data.VpiMax = 780;
            Data.VpiMin = 0;
            Data.TolerancePercentVpi = 0.25;
            Points = new ObservableCollection<PointConfigViewModel>();
            Units = UnitDict.GetUnitsForType(ChannelType.Pressure);
            Data.Unit = Units.FirstOrDefault();
            OutputRanges = new[]
            {
                OutGange.I4_20mA,
                OutGange.I0_5mA,
            };
            Data.OutputRange = OutputRanges.FirstOrDefault();
            _tolerancePercentSigma = Data.TolerancePercentSigma;
            _tolerancePercentSigmaStr = _tolerancePercentSigma.ToString();
            _tolerancePercentVpi = Data.TolerancePercentVpi;
            _tolerancePercentVpiStr = _tolerancePercentVpi.ToString();
            PointCalculator = new CheckPointVm();
            _vpiMin = Data.VpiMin;
            _vpiMinStr = _vpiMin.ToString();
            _vpiMax = Data.VpiMax;
            _vpiMaxStr = _vpiMin.ToString();
            UpdatePoints();
        }

        /// <summary>
        /// Фактические данные конфигурации
        /// </summary>
        /// <remarks>
        /// Использовать на разметке экрана только в случае единственного места изменения, так как без INotifyPropertyChanged
        /// </remarks>
        public PressureSensorConfig Data { get; }

        /// <summary>
        /// Минимум диапазона
        /// </summary>
        public string VpiMinStr
        {
            get { return _vpiMinStr; }
            set
            {
                _vpiMinStr = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                VpiMin = dval;
                OnPropertyChanged(nameof(VpiMinStr));
            }
        }

        /// <summary>
        /// Максимум диапазона
        /// </summary>
        public string VpiMaxStr
        {
            get { return _vpiMaxStr; }
            set
            {
                _vpiMaxStr = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                VpiMax = dval;
                OnPropertyChanged(nameof(VpiMaxStr));
            }
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
                OnSettedVpiMax(_vpiMax);
                OnPropertyChanged(nameof(VpiMax));

                Data.VpiMax = _vpiMax;
                UpdatePoints();
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
                OnSettedVpiMin(_vpiMin);
                OnPropertyChanged(nameof(VpiMin));

                Data.VpiMin = _vpiMin;
                UpdatePoints();
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
            get { return _pressureUnit; }
            set
            {
                if (value == _pressureUnit)
                    return;
                _pressureUnit = value;
                OnSelectedUnit(_pressureUnit);
                OnPropertyChanged("PressUnit");


                Data.Unit = _pressureUnit;
                foreach (var point in Points)
                {
                    point.Unit = _pressureUnit;
                }
            }
        }

        /// <summary>
        /// Допуск по приведенной погрешности
        /// </summary>
        public string TolerancePercentSigma
        {
            get { return _tolerancePercentSigmaStr; }
            set
            {
                _tolerancePercentSigmaStr = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _tolerancePercentSigma = dval;
                Data.TolerancePercentSigma = _tolerancePercentSigma;
                UpdatePoints();
                //OnPropertyChanged("TolerancePercentSigma");
            }
        }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public string TolerancePercentVpi
        {
            get { return _tolerancePercentVpiStr; }
            set
            {
                _tolerancePercentVpiStr = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _tolerancePercentVpi = dval;
                Data.TolerancePercentVpi = _tolerancePercentVpi;
                UpdatePoints();
                //OnPropertyChanged("TolerancePercentVpi");
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
            get { return _outputRange; }
            set
            {
                if (value == _outputRange)
                    return;
                _outputRange = value;
                OnSelectedRange(_outputRange);
                OnPropertyChanged(nameof(OutputRange));

                Data.OutputRange = _outputRange;
                UpdatePoints();
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
        /// Перерассчитать точки
        /// </summary>
        private void UpdatePoints()
        {
            var Pmax = Data.VpiMax;
            var Pmin = Data.VpiMin;
            if (Pmin >= Pmax)
                return;

            var step = (Pmax - Pmin) / (_pointsOnRange - 1);
            double uMin = 0.0;
            double uMax = 5.0;
            if (Data.OutputRange == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }

            Points.Clear();
            Data.Points.Clear();
            for (double i = 0; i < _pointsOnRange; i++)
            {
                var point = Pmin + (i * step);

                var pointOut = CheckPressureLogicConfig.CalcRes(point, Pmin, Pmax, uMin, uMax, Data.TolerancePercentVpi,
                    Data.TolerancePercentSigma);
                var sensPoint = new PressureSensorPoint()
                {
                    PressurePoint = point,
                    OutPoint = pointOut.Ip,
                    PressureUnit = Data.Unit,
                    OutUnit = KipTM.Interfaces.Units.mA,
                    Tollerance = pointOut.dIp,
                };
                Data.Points.Add(sensPoint);
                Points.Add(new PointConfigViewModel(_context, point, pointOut.Ip, pointOut.dIp, Data.Unit));
            }
            PointCalculator.TolerancePercentVpi = Data.TolerancePercentVpi;
            PointCalculator.TolerancePercentSigma = Data.TolerancePercentSigma;
            PointCalculator.Imax = uMax;
            PointCalculator.Imin = uMin;
            PointCalculator.Pmax = Pmax;
            PointCalculator.Pmin = Pmin;
            PointCalculator.UpdateFormulas();
        }

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
            _vpiMax = vpiMax;
            _vpiMin = vpiMin;
            Units = units;
            _pressureUnit = pressureUnit;
            OutputRanges = outputRanges;
            _outputRange = range;
            _tolerancePercentVpi = toleranceVpi;
            _tolerancePercentSigma = toleranceSigma;
            PointCalculator = new CheckPointVm();
            PointCalculator.TolerancePercentVpi = _tolerancePercentVpi;
            PointCalculator.TolerancePercentSigma = _tolerancePercentSigma;
            PointCalculator.Imax = uMax;
            PointCalculator.Imin = uMin;
            PointCalculator.Pmax = vpiMax;
            PointCalculator.Pmin = vpiMin;
            PointCalculator.UpdateFormulas();
        }

        public event Action<double> SettedVpiMax;
        public event Action<double> SettedVpiMin;
        public event Action<Units> SelectedUnit;
        public event Action<OutGange> SelectedRange;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        protected virtual void OnSettedVpiMax(double obj)
        {
            SettedVpiMax?.Invoke(obj);
        }

        protected virtual void OnSettedVpiMin(double obj)
        {
            SettedVpiMin?.Invoke(obj);
        }

        protected virtual void OnSelectedUnit(Units obj)
        {
            SelectedUnit?.Invoke(obj);
        }

        protected virtual void OnSelectedRange(OutGange obj)
        {
            SelectedRange?.Invoke(obj);
        }

        public PointConfigViewModel AddPoint(double pressure, double i, double di, Units unit)
        {
            var point = new PointConfigViewModel(_context, pressure, i, di, unit);
            _context.Invoke(()=>Points.Add(point));
            return point;
        }
    }
}