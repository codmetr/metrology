using System.Collections.Generic;
using System.Linq;

namespace KipTM.Archive
{
    public class DictionariesPool
    {
        public static string DeviceTypesKey { get { return "DeviceTypes"; } }
        public static string FormsTOKey { get { return "FormsTO"; } }
        public static string AircraftTypesKey { get { return "AircraftTypes"; } }
        public static string UsersKey { get { return "Users"; } }

        public DictionariesPool()
        {
            DeviceTypes = new List<string>();
            FormsTO = new List<string>();
            AircraftTypes = new List<string>();
            Users = new List<string>();
        }

        /// <summary>
        /// Типы устройств
        /// </summary>
        public List<string> DeviceTypes { get; set; }
        /// <summary>
        /// Формы технического обслуживания
        /// </summary>
        public List<string> FormsTO { get; set; }
        /// <summary>
        /// Типы воздужных судов
        /// </summary>
        public List<string> AircraftTypes { get; set; }
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

            // Заполнение списка формы технического обслуживания
            tempElement = archive.Data.First(el => el.Key == FormsTOKey);
            if (tempElement != null)
            {
                if (tempElement.Value is List<string>)
                    res.FormsTO = tempElement.Value as List<string>;
            }

            // Заполнение списка типов воздужных судов
            tempElement = archive.Data.First(el => el.Key == AircraftTypesKey);
            if (tempElement != null)
            {
                if (tempElement.Value is List<string>)
                    res.AircraftTypes = tempElement.Value as List<string>;
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
