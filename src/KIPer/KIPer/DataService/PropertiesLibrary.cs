using System.Collections.Generic;
using System.Linq;
using CheckFrame;
using KipTM.Archive;
using KipTM.Interfaces;
using KipTM.Interfaces.Archive;

namespace KipTM.Model
{
    /// <summary>
    /// Библиотека словарей
    /// </summary>
    public class PropertiesLibrary : IPropertiesLibrary
    {
        private const string PathProperties = "prop.xml";
        //private const string PathArchive = "archive.xml";
        private const string PathDictionaries = "dictionaries.xml";

        private readonly DataPool _propertyPool;
        //private readonly ArchiveBase _checksArchive;
        //private readonly ChecksPool _checksPool;
        private readonly ArchiveBase _dictionariesArchive;
        private readonly DictionariesPool _dictionariesPool;


        public PropertiesLibrary(IEnumerable<IArchiveDataDefault> defaultFactories, FeatureDescriptorsCombiner features)
        {
            var def = new ArchiveBase(defaultFactories.SelectMany(el => el.GetDefaultData()).ToList());
            _propertyPool = new DataPool(ArchiveBase.LoadFromFile(PathProperties, def));
            //_propertyPool = new DataPool(ArchiveBase.LoadFromFile(PathProperties, PropertyArchive.GetDefault()));

            //var devices = features.SelectMany(el => el.GetDefaultForCheckTypes()).ToList();
            var devices = features.GetDefaultForCheckTypes().ToList();
            //_dictionariesArchive = ArchiveBase.LoadFromFile(PathDictionaries, DictionariesArchive.GetDefault(devices));
            _dictionariesArchive = DictionariesArchive.GetDefault(devices);
            _dictionariesPool = DictionariesPool.Load(_dictionariesArchive);
            //_checksArchive = ArchiveBase.LoadFromFile(PathArchive, new ArchiveBase());
            //_checksPool = ChecksPool.Load(_checksArchive);
            //_checksArchive = ArchiveBase.LoadFromFile(PathArchive, new ArchiveBase());
        }

        /// <summary>
        /// Список настроек хода проверок
        /// </summary>
        public DataPool PropertyPool
        {
            get { return _propertyPool; }
        }

        //public ArchiveBase ChecksArchive
        //{
        //    get { return _checksArchive; }
        //}

        //public ChecksPool ChecksPool
        //{
        //    get { return _checksPool; }
        //}

        /// <summary>
        /// Список словарей (имена пользователей, организации и пр.)
        /// </summary>
        public DictionariesPool DictionariesPool
        {
            get { return _dictionariesPool; }
        }
    }
}
