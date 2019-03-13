using KipTM.Interfaces;

namespace PressureSensorData
{
    /// <summary>
    /// Результат измерений на точке проверки
    /// </summary>
    public class PressureSensorPointResult
    {
        public PressureSensorPointResult()
        {
            
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
        /// Точка напряжения
        /// </summary>
        public double VoltagePoint { get; set; }
        /// <summary>
        /// Единицы измерения напряжения
        /// </summary>
        public Units VoltageUnit { get; set; }

        /// <summary>
        /// Результат: давление
        /// </summary>
        public double PressureValue { get; set; }
        /// <summary>
        /// Результат: напряжение
        /// </summary>
        public double OutPutValue { get; set; }
        /// <summary>
        /// Результат: напряжение на обратном ходе
        /// </summary>
        public double OutPutValueBack { get; set; }

        /// <summary>
        /// Результат: в допуске
        /// </summary>
        public bool IsCorrect { get; set; }
        /// <summary>
        /// Результат: в допуске на обратном ходе
        /// </summary>
        public bool IsCorrectBack { get; set; }
    }
}