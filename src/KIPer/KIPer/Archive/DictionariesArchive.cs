using System.Collections.Generic;
using System.Linq;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    class DictionariesArchive:ArchiveBase
    {
        #region GetDefault
        public static ArchiveBase GetDefault(List<ArchivedKeyValuePair> devices)
        {
            return new ArchiveBase(GetDefaultData(devices));
        }

        private static List<ArchivedKeyValuePair> GetDefaultData(List<ArchivedKeyValuePair> devices)
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(DictionariesPool.DeviceTypesKey, devices.Select(el=>el.Key)),
                new ArchivedKeyValuePair(DictionariesPool.CheckTypesKey, devices.Where(el =>
                {
                    var checks = el.Value as List<string>;
                    return checks == null ? false : checks.Count > 0;
                }).ToList()),
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
