using KipTM.Archive;

namespace KipTM.Model
{
    public class ArchiveService
    {
        private const string PathProperties = "prop.xml";
        private const string PathArchive = "archive.xml";
        private const string PathDictionaries = "dictionaries.xml";

        private readonly DataPool _propertyPool;
        private readonly ArchiveBase _checksArchive;
        private readonly ChecksPool _checksPool;
        private readonly ArchiveBase _dictionariesArchive;
        private readonly DictionariesPool _dictionariesPool;


        public ArchiveService()
        {
            _propertyPool = new DataPool(ArchiveBase.LoadFromFile(PathProperties, PropertyArchive.GetDefault()));
            _checksArchive = ArchiveBase.LoadFromFile(PathArchive, new ArchiveBase());
            _checksPool = ChecksPool.Load(_checksArchive);
            _checksArchive = ArchiveBase.LoadFromFile(PathArchive, new ArchiveBase());
            _dictionariesArchive = ArchiveBase.LoadFromFile(PathDictionaries, DictionariesArchive.GetDefault());
            _dictionariesPool = DictionariesPool.Load(_dictionariesArchive);
        }

        public DataPool PropertyPool
        {
            get { return _propertyPool; }
        }

        public ArchiveBase ChecksArchive
        {
            get { return _checksArchive; }
        }

        public ChecksPool ChecksPool
        {
            get { return _checksPool; }
        }

        public DictionariesPool DictionariesPool
        {
            get { return _dictionariesPool; }
        }
    }
}
