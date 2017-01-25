using System.Collections.Generic;
using System.Drawing;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    /// <summary>
    /// Пулл методик
    /// </summary>
    public interface IMethodsService
    {
        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        IDictionary<string, ICheckMethod> MethodsForType(string deviceKey);

        /// <summary>
        /// Получить набор ключей
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetKeys();

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        string GetTitle(string key);

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        Bitmap GetBigImage(string key);

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        Bitmap GetSmallImage(string key);
    }
}