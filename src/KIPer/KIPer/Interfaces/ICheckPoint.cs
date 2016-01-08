using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.Interfaces
{
    /// <summary>
    /// Описатель одной точки проверки
    /// </summary>
    public interface ICheckPoint
    {
        /// <summary>
        /// Точка проверки (фактическая величина)
        /// </summary>
        string Point { get; }
        /// <summary>
        /// Единица измерения параметра
        /// </summary>
        string Units { get; }
        /// <summary>
        /// Допустимая погрешность
        /// </summary>
        string Tolerance { get; }
    }
}
