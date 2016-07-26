﻿using System;
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

        /// <summary>
        /// Получить представление(маркер) заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<IParameterResultViewModel> GetMarkers(Type T, object item);
    }
}