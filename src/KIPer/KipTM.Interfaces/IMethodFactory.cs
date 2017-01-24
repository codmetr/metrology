using System;
using System.Collections.Generic;
using System.Drawing;
using KipTM.Model.Checks;

namespace KipTM.Interfaces
{
    /// <summary>
    /// Фабрика методик
    /// </summary>
    public interface IMethodFactory
    {
        /// <summary>
        /// Получить ключ устройства для типа оборудования
        /// </summary>
        string GetKey();
        
        /// <summary>
        /// Получить набор методик для конкретного типа оборудования
        /// </summary>
        /// <returns>
        /// Dictionary"ключ методики", "методика"
        /// </returns>
        Dictionary<string, ICheckMethod> GetMethods();

        /// <summary>
        /// Получить большое изображение целевого устройства
        /// </summary>
        /// <returns></returns>
        Bitmap GetBigImage();

        /// <summary>
        /// Получить малое изображение целевого устройства
        /// </summary>
        /// <returns></returns>
        Bitmap GetSmallImage();

        /// <summary>
        /// Получить заголовок проверок целевого устройства
        /// </summary>
        /// <returns></returns>
        string GetName();

    }
}