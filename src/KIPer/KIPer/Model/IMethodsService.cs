using System.Collections.Generic;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    public interface IMethodsService
    {
        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        IDictionary<string, ICheckMethod> MethodsForType(string DeviceKey);
    }
}