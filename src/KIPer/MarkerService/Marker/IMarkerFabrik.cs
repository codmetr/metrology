using System;
using System.Collections.Generic;

namespace MarkerService
{
    public interface IMarkerFabrik<T>
    {
        /// <summary>
        /// Получить маркер результата заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<T> GetMarkers<Ttarget>(Ttarget item);

        /// <summary>
        /// Получить маркер заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<T> GetMarkers(Type Ttarget, object item);
    }
}