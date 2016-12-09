using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Interfaces.Archive
{
    /// <summary>
    /// ‘абрика значений по умолчанию дл€ набора данных архива
    /// </summary>
    public interface IArchiveDataDefault
    {
        /// <summary>
        /// ѕолучить набор данных по умолчанию
        /// </summary>
        /// <returns></returns>
        List<ArchivedKeyValuePair> GetDefaultData();
    }
}