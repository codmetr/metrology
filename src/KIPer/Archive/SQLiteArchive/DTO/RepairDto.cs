using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    /// <summary>
    /// Идентификатор проверки
    /// </summary>
    public class RepairDto
    {
        /// <summary>
        /// Идентификатор проверки
        /// </summary>
        public int RepairId { get; set; }

        /// <summary>
        /// Метка времени создания
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Метка времени создания
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Ключ типа проверяемого устройства
        /// </summary>
        public string TargetDeviceKey { get; set; }

        /// <summary>
        /// Tипа проверяемого устройства
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Серийны номер
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Общий результат проверки
        /// </summary>
        public string CommonResult { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }
    }
}
