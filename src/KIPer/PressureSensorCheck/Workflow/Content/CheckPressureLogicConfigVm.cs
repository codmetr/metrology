﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация логики проверки
    /// </summary>
    public class CheckPressureLogicConfigVm : INotifyPropertyChanged
    {
        private double _pointsOnRange = 5;

        public CheckPressureLogicConfigVm(PressureSensorConfig data)
        {
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
        /// Максимум диапазона
        /// </summary>
        public double VpiMax
        {
            get { return Data.VpiMax; }
            set
            {
                Data.VpiMax = value;
                UpdatePoints();
                OnPropertyChanged("VpiMax");
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
                OnPropertyChanged("VpiMin");
            }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public IEnumerable<Units> Units { get; set; }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public double TolerancePercentVpi
        {
            get { return Data.TolerancePercentVpi; }
            set
            {
                Data.TolerancePercentVpi = value;
                UpdatePoints();
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
            get { return Data.OutputRange; }
            set
            {
                if (value == Data.OutputRange)
                    return;
                Data.OutputRange = value;
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
            var max = Data.VpiMax;
            var min = Data.VpiMin;
            var tollerance = Data.TolerancePercentVpi;
            if (tollerance <= 0)
                return;
            if (min >= max)
                return;

            var step = (max - min) / (_pointsOnRange - 1);
            var du = (max - min) * tollerance / 100.0;
            double uMin = 0.0;
            double uMax = 5.0;
            if (Data.OutputRange == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }
            double uStep = (uMax - uMin) / (_pointsOnRange - 1);

            Points.Clear();
            Data.Points.Clear();
            for (double i = 0; i < _pointsOnRange; i++)
            {
                var point = min + (i * step);
                var uPoint = uMin + (i * uStep);
                var sensPoint = new PressureSensorPoint()
                {
                    PressurePoint = point,
                    OutPoint = uPoint,
                    PressureUnit = Data.Unit,
                    OutUnit = KipTM.Interfaces.Units.mA,
                    Tollerance = tollerance
                };
                Data.Points.Add(sensPoint);
                Points.Add(new PointConfigViewModel()//TODO добавить связь с sensPoint
                    { Pressure = point, I = uPoint, Unit = Data.Unit, dI = tollerance });
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
}