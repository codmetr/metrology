namespace PACEChecks.Devices
{
    /// <summary>
    /// Результат опроса устройства
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Удался или нет запрос
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Подробности ошибки
        /// </summary>
        public string ErrorNote { get; set; }

        /// <summary>
        /// Дополнительные данные ошибки
        /// </summary>
        public object Arg { get; set; }
    }
}
