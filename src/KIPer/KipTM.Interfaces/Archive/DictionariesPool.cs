using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    /// <summary>
    /// Справочник
    /// </summary>
    public class DictionariesPool
    {
        public static string DeviceTypesKey { get { return "DeviceTypes"; } }
        public static string CheckTypesKey { get { return "CheckTypes"; } }
        public static string UsersKey { get { return "Users"; } }

        public DictionariesPool()
        {
            CheckTypes = new Dictionary<DeviceTypeDescriptor, IEnumerable<string>>();
            DeviceTypes = CheckTypes.Keys.ToList();
            Users = new List<string>();
        }

        /// <summary>
        /// Типы устройств
        /// </summary>
        public List<DeviceTypeDescriptor> DeviceTypes { get; set; }

        /// <summary>
        /// Справочкник типов поддерживаемых проверок для типа устройства
        /// </summary>
        public IDictionary<DeviceTypeDescriptor, IEnumerable<string>> CheckTypes { get; set; }

        /// <summary>
        /// Пользователи
        /// </summary>
        public List<string> Users { get; set; }

        public static DictionariesPool Load(IDictionary<DeviceTypeDescriptor, IEnumerable<string>> devDictionaryes,  ArchiveBase archive)
        {
            var res = new DictionariesPool();

            // Заполнение списка типов устройств
            res.CheckTypes = devDictionaryes;
            res.DeviceTypes = devDictionaryes.Keys.ToList();
            
            // Заполнение списка пользователи
            var tempElement = archive.Data.First(el => el.Key == UsersKey);
            if (tempElement != null)
            {
                if (tempElement.Value is List<string>)
                    res.Users = tempElement.Value as List<string>;
            }

            return res;
        }
    }
}
