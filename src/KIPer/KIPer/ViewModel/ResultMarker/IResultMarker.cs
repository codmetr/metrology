using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.ResultMarker
{
    /// <summary>
    /// Получает описатель объекта как параметра результата для вывода пользователю
    /// </summary>
    interface IResultMarker
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>писатель результата</returns>
        IEnumerable<IParameterResultViewModel> Make(object target, IResultMarkerFabrik markerFabric);
    }
}
