using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPI620Genii
{
    /// <summary>
    /// Лог измерения с DPI620Genii
    /// </summary>
    public interface IDpi620Genii
    {
        string FileName { get;}
        DateTime StartTime { get;}

        /// <summary>
        /// Получить очередную запись
        /// </summary>
        /// <returns>Запись об измерениях</returns>
        Record GetNextRecord();
    }
}
