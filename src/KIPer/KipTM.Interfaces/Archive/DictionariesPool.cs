using System.Collections.Generic;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;

namespace KipTM.Interfaces.Archive
{
    /// <summary>
    /// Справочник
    /// </summary>
    public class DictionariesPool: IDictionaryPool
    {
        public static string DeviceTypesKey { get { return "DeviceTypes"; } }
        public static string CheckTypesKey { get { return "CheckTypes"; } }

        public DictionariesPool()
        {
            CheckTypes = new Dictionary<DeviceTypeDescriptor, IEnumerable<string>>();
            DeviceTypes = CheckTypes.Keys.ToList();
        }

        /// <summary>
        /// Типы устройств
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; set; }

        /// <summary>
        /// Справочкник типов поддерживаемых проверок для типа устройства
        /// </summary>
        public IDictionary<DeviceTypeDescriptor, IEnumerable<string>> CheckTypes { get; set; }

        public static DictionariesPool Load(IDictionary<DeviceTypeDescriptor, IEnumerable<string>> devDictionaryes)
        {
            var res = new DictionariesPool();

            // Заполнение списка типов устройств
            res.CheckTypes = devDictionaryes;
            res.DeviceTypes = devDictionaryes.Keys.ToList();

            return res;
        }
    }
}
