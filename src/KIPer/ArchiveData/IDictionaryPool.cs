using System.Collections.Generic;
using ArchiveData.DTO;

namespace ArchiveData
{
    public interface IDictionaryPool
    {
        /// <summary>
        /// Получить список типов проверяемого оборудования из справочника
        /// </summary>
        IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; }
    }
}