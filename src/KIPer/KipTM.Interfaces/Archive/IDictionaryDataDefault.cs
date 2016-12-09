using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Interfaces.Archive
{
    /// <summary>
    /// Перечень словарей по умолчанию
    /// </summary>
    public interface IDictionaryDataDefault
    {
        /// <summary>
        /// Получить набор данных по умолчанию
        /// </summary>
        /// <returns></returns>
        List<ArchivedKeyValuePair> GetDefaultData(); 
    }
}