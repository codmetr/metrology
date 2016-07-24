using System.Collections.Generic;

namespace KipTM.ViewModel.ResultMarker
{
    public interface IResultMarkerFabrik
    {
        /// <summary>
        /// Получить представление(маркер) результата заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<IParameterResultViewModel> GetMarkers<T>(T item);
    }
}