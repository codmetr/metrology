using System.Collections.Generic;
using CheckFrame.Model.Checks;
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