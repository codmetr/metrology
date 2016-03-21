using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    class DictionariesArchive:ArchiveBase
    {
        public static ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        private static List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(DictionariesPool.AircraftTypesKey, GetDefaultForAircraftTypes()),
                new ArchivedKeyValuePair(DictionariesPool.DeviceTypesKey, GetDefaultForDeviceTypes()),
                new ArchivedKeyValuePair(DictionariesPool.FormsTOKey, GetDefaultForFormsTO()),
                new ArchivedKeyValuePair(DictionariesPool.UsersKey, GetDefaultForUsers()),
            };
        }

        private static object GetDefaultForUsers()
        {
            return new List<string>()
            {
                "Иванов Иван Иванович",
                "Петров Петр Петрович",
                "Сидоров Сидор Сидорович",
            };
        }

        private static object GetDefaultForFormsTO()
        {
            return new List<string>()
            {
                "TO1",
                "ТО2",
                "ТО3",
            };
        }

        private static object GetDefaultForDeviceTypes()
        {
            return new List<string>()
            {
                "СВС-В1-72",
                "СВС-В1-72",
                "СВС-В1-72",
            };
        }

        private static object GetDefaultForAircraftTypes()
        {
            return new List<string>()
            {
                "Ил-76",
                "Ил-76КТ",
                "Ил-76ЛЛ",
            };
        }
    }
}
