using System.Collections.Generic;
using System.Linq;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public class ChecksPool
    {
        private const string ResultsKey = "Results";

        private readonly ArchiveBase _archive;

        private Dictionary<string, CheckData> _checks;

        private ChecksPool(ArchiveBase archive, Dictionary<string, CheckData> checks)
        {
            _archive = archive;
            _checks = checks;
        }

        public static ChecksPool Load(ArchiveBase archive)
        {
            var checks = new Dictionary<string, CheckData>();
            foreach (var keyValuePair in archive.Data)
            {
                var element = keyValuePair.Value as List<ArchivedKeyValuePair>;

                if(element==null)
                    continue;
                
                if (element.All(el=>el.Key != CheckKey.KeyString))
                    continue;

                var elementArchive = archive.GetArchive(keyValuePair.Key);
                
                // заполнение ключа проверки
                var checkKeyArchive = elementArchive.GetArchive(CheckKey.KeyString);
                var checkKeyPool = new DataPool(checkKeyArchive);
                var checkKey = CheckKey.Load(checkKeyPool);
                
                // создание экземпляра проверки
                var check = new CheckData(checkKey);

                // заполнение метаданных проверки
                if (elementArchive.Data.Any(el => el.Key == CheckMetadata.KeyString))
                {
                    var metadataArchive = elementArchive.GetArchive(CheckMetadata.KeyString);
                    var metadataPool = new DataPool(metadataArchive);
                    var metadata = CheckMetadata.Load(metadataPool);
                    check.Metadata = metadata;
                }

                // заполненине результатов проверки
                if (elementArchive.Data.Any(el => el.Key == ResultsKey))
                {
                    var resultsArchive = elementArchive.GetArchive(ResultsKey);
                    var resultsPool = new DataPool(resultsArchive);
                    var results = CheckData.LoadResults(resultsPool);
                    check.Results = results;
                }

                checks.Add(keyValuePair.Key, check);
            }
            return new ChecksPool(archive, checks);
        }

        public Dictionary<string, CheckData> Checks{get { return _checks; }}
        
    }
}
