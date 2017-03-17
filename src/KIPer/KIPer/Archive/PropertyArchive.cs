using System.Collections.Generic;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces.Archive;
using KipTM.Model.Devices;

namespace KipTM.Archive
{
    /*TODO вспомнить как это использовать
    public class PropertyArchive : ArchiveBase
    {

        public PropertyArchive()
            : base()
        { }

        public PropertyArchive(List<ArchivedKeyValuePair> data)
            : base(data)
        { }

        #region GetDefault
        public static ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        private static readonly IEnumerable<IArchiveDataDefault> _listDataFactory = null;
        static PropertyArchive()
        {
            _listDataFactory = GetFactories();
        }

        private static IEnumerable<IArchiveDataDefault> GetFactories()
        {
            //TODO заполнение из IOC
            var adtsDefault = new ArchiveDataAdts();
            return new List<IArchiveDataDefault> { adtsDefault };
        }

        public static List<ArchivedKeyValuePair> GetDefaultData()
        {

            var data = new List<ArchivedKeyValuePair>();
            foreach (var dataDefault in _listDataFactory)
            {
                data.AddRange(dataDefault.GetDefaultData());
            }
            
            return data;
        }
        #endregion
    }*/
}
