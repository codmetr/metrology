using System.Collections.Generic;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using KipTM.Archive.DataTypes;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Archive
{
    class DictionariesArchive:ArchiveBase
    {
        #region GetDefault
        public static ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        private static List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(DictionariesPool.DeviceTypesKey, GetDefaultForDeviceTypes()),
                new ArchivedKeyValuePair(DictionariesPool.CheckTypesKey, GetDefaultForCheckTypes()),
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

        private static object GetDefaultForCheckTypes()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
                {
                    AdtsCheckMethod.Key,
                }),
            };
        }

        private static object GetDefaultForDeviceTypes()
        {
            return new List<string>()
            {
                ADTSModel.Key,
            };
        }
        #endregion
    }
}
