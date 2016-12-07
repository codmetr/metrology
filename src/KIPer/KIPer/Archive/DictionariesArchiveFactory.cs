using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    class DictionariesArchiveFactory
    {
        #region GetDefault
        public ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData(null));
        }

        private List<ArchivedKeyValuePair> GetDefaultData(IEnumerable<IDictionariesArchiveFactory> dics)
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(DictionariesPool.DeviceTypesKey, GetDefaultForDeviceTypes(dics)),
                new ArchivedKeyValuePair(DictionariesPool.CheckTypesKey, GetDefaultForCheckTypes(dics)),
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

        private object GetDefaultForCheckTypes(IEnumerable<IDictionariesArchiveFactory> dics)
        {
            var result = new List<ArchivedKeyValuePair>();
            foreach (var archiveFactory in dics)
            {
                result.AddRange(archiveFactory.GetDefaultForCheckTypes());
            }
            //{
            //    new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
            //    {
            //        Calibration.Key,
            //    }),
            //};
            return result;
        }

        private object GetDefaultForDeviceTypes(IEnumerable<IDictionariesArchiveFactory> dics)
        {
            var result = new List<string>();
            foreach (var archiveFactory in dics)
            {
                result.AddRange(archiveFactory.GetDefaultForDeviceTypes());
            }
            //return new List<string>()
            //{
            //    ADTSModel.Key,
            //};
            return result;
        }
        #endregion
    }
}
