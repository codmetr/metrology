using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public interface IDictionariesArchiveFactory
    {
        IEnumerable<ArchivedKeyValuePair> GetDefaultForCheckTypes();
        IEnumerable<string> GetDefaultForDeviceTypes();
    }
}