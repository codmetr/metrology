using System.Collections.Generic;
using System.Drawing;
using ArchiveData.DTO;
using KipTM.Interfaces.Checks;
using KipTM.Model.Checks;

namespace KipTM.Interfaces
{
    /// <summary>
    /// Пулл методик
    /// </summary>
    public interface IMethodsService
    {
        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        IDictionary<string, ICheckMethod> MethodsForType(DeviceTypeDescriptor deviceKey);

        /// <summary>
        /// Получить описатели устройств
        /// </summary>
        /// <returns></returns>
        IEnumerable<DeviceViewDescriptor> GetDescriptors();
    }
}