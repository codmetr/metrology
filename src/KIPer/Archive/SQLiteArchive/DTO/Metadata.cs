using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    /// <summary>
    /// Метаданные
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Ключ данных
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Данные
        /// </summary>
        public string Data { get; set; }
    }
}
