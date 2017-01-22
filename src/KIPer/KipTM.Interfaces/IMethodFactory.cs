using System;
using System.Collections.Generic;
using KipTM.Model.Checks;

namespace KipTM.Interfaces
{
    /// <summary>
    /// Фабрика методик
    /// </summary>
    public interface IMethodFactory
    {
        /// <summary>
        /// Получить набор методик для конкретного типа оборудования
        /// </summary>
        /// <returns>
        /// Tuple"ключ устройства", Dictionary"ключ методики", "методика"
        /// </returns>
        Tuple<string, Dictionary<string, ICheckMethod>> GetMethod();
    }
}