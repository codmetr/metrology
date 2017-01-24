using System.Collections.Generic;
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
    }
}