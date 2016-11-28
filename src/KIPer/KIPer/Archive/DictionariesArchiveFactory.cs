using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    class DictionariesArchiveFactory
    {
        #region GetDefault
        public ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        private List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(DictionariesPool.DeviceTypesKey, GetDefaultForDeviceTypes()),
                new ArchivedKeyValuePair(DictionariesPool.CheckTypesKey, GetDefaultForCheckTypes()),
                new ArchivedKeyValuePair(DictionariesPool.UsersKey, GetDefaultForUsers()),
            };
        }

        private object GetDefaultForUsers()
        {
            return new List<string>()
            {
                "Иванов Иван Иванович",
                "Петров Петр Петрович",
                "Сидоров Сидор Сидорович",
            };
        }

        private object GetDefaultForCheckTypes()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
                {
                    Calibration.Key,
                }),
            };
        }

        private object GetDefaultForDeviceTypes()
        {
            return new List<string>()
            {
                ADTSModel.Key,
            };
        }
        #endregion
    }
}
