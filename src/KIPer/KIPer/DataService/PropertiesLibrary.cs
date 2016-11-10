using KipTM.Archive;

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


        public PropertiesLibrary(PropertyArchive prorArchive)
        {
            _propertyPool = new DataPool(ArchiveBase.LoadFromFile(PathProperties, PropertyArchive.GetDefault()));
            _dictionariesArchive = ArchiveBase.LoadFromFile(PathDictionaries, DictionariesArchive.GetDefault());
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
