using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public interface IArchiveDataDefault
    {
        List<ArchivedKeyValuePair> GetDefaultData();
    }
}