using System.Collections.Generic;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    public interface IMethodicsService
    {
        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        IDictionary<string, ICheckMethodic> MethodicsForType(string DeviceKey);
    }
}