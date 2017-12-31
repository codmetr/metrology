namespace PressureSensorData
{
    /// <summary>
    /// Точка проверки преобразования давления в напряжение
    /// </summary>
    public class PressureSensorPoint
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
        /// Допуск по напряжению
        /// </summary>
        public double Tollerance { get; set; }
    }
}
