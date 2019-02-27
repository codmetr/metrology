using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow.Content
{
    class CheckPressureLogicConfig
    {
        private readonly CheckPressureLogicConfigVm _vm;
        private string _tolerancePercentSigma;
        private string _tolerancePercentVpi;
        private double _pointsOnRange = 5;

        private string _vpiMinStr;
        private string _vpiMaxStr;

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfig(PressureSensorConfig data, CheckPressureLogicConfigVm vm)
        {
            Data = data;
            _vm = vm;
            Data.VpiMax = 780;
            Data.VpiMin = 0;
            Data.TolerancePercentVpi = 0.25;
            Units = UnitDict.GetUnitsForType(ChannelType.Pressure);
            Data.Unit = Units.FirstOrDefault();
            OutputRanges = new[]
            {
                OutGange.I4_20mA,
                OutGange.I0_5mA,
            };
            Data.OutputRange = OutputRanges.FirstOrDefault();
            _tolerancePercentSigma = Data.TolerancePercentSigma.ToString();
            _tolerancePercentVpi = Data.TolerancePercentVpi.ToString();
            PointCalculator = new CheckPointVm();
            _vpiMinStr = Data.VpiMin.ToString();
            _vpiMaxStr = Data.VpiMax.ToString();
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
            }
        }

        /// <summary>
        /// Максимум диапазона
        /// </summary>
        public double VpiMax
        {
            get { return Data.VpiMax; }
            set
            {
                Data.VpiMax = value;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Минимум диапазона
        /// </summary>
        public double VpiMin
        {
            get { return Data.VpiMin; }
            set
            {
                Data.VpiMin = value;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public IEnumerable<Units> Units { get; set; }

        /// <summary>
        /// Допуск по приведенной погрешности
        /// </summary>
        public string TolerancePercentSigma
        {
            get { return _tolerancePercentSigma; }
            set
            {
                _tolerancePercentSigma = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                Data.TolerancePercentSigma = dval;
                UpdatePoints();
                //OnPropertyChanged("TolerancePercentSigma");
            }
        }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public string TolerancePercentVpi
        {
            get { return _tolerancePercentVpi; }
            set
            {
                _tolerancePercentVpi = value;
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                Data.TolerancePercentVpi = dval;
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
            get { return Data.OutputRange; }
            set
            {
                if (value == Data.OutputRange)
                    return;
                Data.OutputRange = value;
                UpdatePoints();
            }
        }

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

            Data.Points.Clear();
            for (double i = 0; i < _pointsOnRange; i++)
            {
                var point = Pmin + (i * step);

                var pointOut = CheckPointVm.CalcRes(point, Pmin, Pmax, uMin, uMax, Data.TolerancePercentVpi,
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
            }
            PointCalculator.TolerancePercentVpi = Data.TolerancePercentVpi;
            PointCalculator.TolerancePercentSigma = Data.TolerancePercentSigma;
            PointCalculator.Imax = uMax;
            PointCalculator.Imin = uMin;
            PointCalculator.Pmax = Pmax;
            PointCalculator.Pmin = Pmin;
            PointCalculator.UpdateFormulas();
        }
    }
}
