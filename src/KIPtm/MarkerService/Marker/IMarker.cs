using System.Collections.Generic;

namespace MarkerService
{
    /// <summary>
    /// Получает описатель объекта как параметра результата для вывода пользователю
    /// </summary>
    public interface IMarker<T>
    {
        /// <summary>
        /// Получить маркер для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <param name="markerFactory"></param>
        /// <returns>маркер</returns>
        IEnumerable<T> Make(object target, IMarkerFactory<T> markerFactory);
    }
}
