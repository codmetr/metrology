﻿using System.Collections.Generic;

namespace PressureSensorData
{
    /// <summary>
    /// Результаты поверки
    /// </summary>
    public class PressureSensorResult
    {
        /// <summary>
        /// Результаты поверки
        /// </summary>
        public PressureSensorResult()
        {
            Points = new List<PressureSensorPoint>();
        }
        /// <summary>
        /// Результат опробирования
        /// </summary>
        public string Assay { get; set; }

        /// <summary>
        /// Результат проверки на герметичность
        /// </summary>
        public string Leak { get; set; }

        /// <summary>
        /// Общий результат поверки
        /// </summary>
        public string CommonResult { get; set; }

        /// <summary>
        /// Результат визуального осмотра
        /// </summary>
        public string VisualCheckResult { get; set; }

        /// <summary>
        /// Результат проверки по точкам
        /// </summary>
        public List<PressureSensorPoint> Points { get; set; }
    }
}
