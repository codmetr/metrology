using System.Collections.Generic;
using System.Linq;

namespace KipTM.Archive
{
    public class DictionariesPool
    {
        public static string DeviceTypesKey { get { return "DeviceTypes"; } }
        public static string CheckTypesKey { get { return "CheckTypes"; } }
        public static string UsersKey { get { return "Users"; } }

        public DictionariesPool()
        {
            DeviceTypes = new List<string>();
            CheckTypes = new Dictionary<string, List<string>>();
            Users = new List<string>();
        }

        /// <summary>
        /// Типы устройств
        /// </summary>
        public List<string> DeviceTypes { get; set; }

        /// <summary>
        /// Справочкник типов поддерживаемых проверок для типа устройства
        /// </summary>
        public Dictionary<string, List<string>> CheckTypes { get; set; }

        /// <summary>
        /// Пользователи
        /// </summary>
        public List<string> Users { get; set; }

        public static DictionariesPool Load(ArchiveBase archive)
        {
            var res = new DictionariesPool();

            // Заполнение списка типов устройств
            var tempElement = archive.Data.First(el => el.Key == DeviceTypesKey);
            if (tempElement != null)
            {
                if (tempElement.Value is List<string>)
                    res.DeviceTypes = tempElement.Value as List<string>;
            }

            // Заполнение справочкника типов поддерживаемых проверок для типа устройства
            var subArchive = archive.GetArchive(CheckTypesKey);
            if (subArchive != null)
            {
                foreach (var pair in subArchive.Data)
                {
                    var devType = pair.Key as string;
                    var checkTypes = pair.Value as List<string>;
                    if (devType != null && checkTypes != null)
                    {
                        if (res.CheckTypes==null)
                            res.CheckTypes = new Dictionary<string, List<string>>();
                        res.CheckTypes.Add(devType, checkTypes);
                    }
                }
            }
            
            // Заполнение списка пользователи
            tempElement = archive.Data.First(el => el.Key == UsersKey);
            if (tempElement != null)
            {
                if (tempElement.Value is List<string>)
                    res.Users = tempElement.Value as List<string>;
            }

            return res;
        }
    }
}
