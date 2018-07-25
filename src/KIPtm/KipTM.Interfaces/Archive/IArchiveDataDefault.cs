using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Interfaces.Archive
{
    /// <summary>
    /// ������� �������� �� ��������� ��� ������ ������ ������
    /// </summary>
    public interface IArchiveDataDefault
    {
        /// <summary>
        /// �������� ����� ������ �� ���������
        /// </summary>
        /// <returns></returns>
        List<ArchivedKeyValuePair> GetDefaultData();
    }
}