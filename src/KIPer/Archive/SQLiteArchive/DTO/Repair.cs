using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    /// <summary>
    /// Идентификатор проверки
    /// </summary>
    public class Repair
    {
        /// <summary>
        /// Идентификатор проверки
        /// </summary>
        public int RepairId { get; set; }

        /// <summary>
        /// Метка времени создания
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
