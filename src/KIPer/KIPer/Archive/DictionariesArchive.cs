using System.Collections.Generic;
using System.Linq;
using KipTM.Archive.DataTypes;

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

        //private static object GetDefaultForCheckTypes(List<ArchivedKeyValuePair> devices)
        //{
        //    return new List<ArchivedKeyValuePair>
        //    {
        //        new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
        //        {
        //            Calibration.Key,
        //        }),
        //    };
        //}

        //private static object GetDefaultForDeviceTypes(List<ArchivedKeyValuePair> devices)
        //{
        //    return new List<string>()
        //    {
        //        ADTSModel.Key,
        //    };
        //}
        #endregion
    }
}
