﻿using System.Collections.Generic;

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
        /// <param name="markerFabric"></param>
        /// <returns>маркер</returns>
        IEnumerable<T> Make(object target, IMarkerFabrik<T> markerFabric);
    }
}
