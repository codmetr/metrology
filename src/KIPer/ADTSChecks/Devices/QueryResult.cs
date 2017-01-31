using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTSChecks.Devices
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
