using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressureSensorData
{
    public class PressureSensorResult
    {
        /// <summary>
        /// Результат проверки
        /// </summary>
        public List<PressureSensorPointResult> Points { get; set; }
    }
}
