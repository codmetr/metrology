namespace PressureSensorData
{
    /// <summary>
    /// Результат точки проверки
    /// </summary>
    public class PressureSensorPoint
    {
        /// <summary>
        /// Индекс точки
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Конфигурация точки
        /// </summary>
        public PressureSensorPointConf Config { get; set; }

        /// <summary>
        /// Результат точки
        /// </summary>
        public PressureSensorPointResult Result { get; set; }
    }
}