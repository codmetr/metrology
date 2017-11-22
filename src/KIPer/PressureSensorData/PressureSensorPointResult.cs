namespace PressureSensorData
{
    public class PressureSensorPointResult
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

        /// <summary>
        /// Результат: давление
        /// </summary>
        public double PressureValue { get; set; }
        /// <summary>
        /// Результат: напряжение
        /// </summary>
        public double VoltageValue { get; set; }
        /// <summary>
        /// Результат: напряжение на обратном ходе
        /// </summary>
        public double VoltageValueBack { get; set; }
    }
}