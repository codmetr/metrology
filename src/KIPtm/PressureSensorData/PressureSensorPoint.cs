﻿using KipTM.Interfaces;

namespace PressureSensorData
{
    /// <summary>
    /// Точка проверки преобразования давления в напряжение
    /// </summary>
    public class PressureSensorPoint
    {
        public PressureSensorPoint DeepCopy()
        {
            return new PressureSensorPoint()
            {
                PressurePoint = PressurePoint,
                PressureUnit = PressureUnit,
                OutPoint = OutPoint,
                OutUnit = OutUnit,
                Tollerance = Tollerance,
            };
        }

        /// <summary>
        /// Точка давления
        /// </summary>
        public double PressurePoint { get; set; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressureUnit { get; set; }

        /// <summary>
        /// Точка выходного сигнала
        /// </summary>
        public double OutPoint { get; set; }

        /// <summary>
        /// Единицы измерения выходного сигнала
        /// </summary>
        public Units OutUnit { get; set; }

        /// <summary>
        /// Допуск по напряжению
        /// </summary>
        public double Tollerance { get; set; }
    }
}
