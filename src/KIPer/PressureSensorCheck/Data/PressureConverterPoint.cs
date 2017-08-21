using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureSensorCheck.Data
{
    /// <summary>
    /// Точка проверки преобразования давления в напряжение
    /// </summary>
    public class PressureConverterPoint
    {
        /// <summary>
        /// Точка давления
        /// </summary>
        public double PressurePoint { get; set; }
        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public string PressureUnit { get; set; }
        /// <summary>
        /// Точка напряжения
        /// </summary>
        public double VoltagePoint { get; set; }
        /// <summary>
        /// Единицы измерения напряжения
        /// </summary>
        public string VoltageUnit { get; set; }
    }
}
