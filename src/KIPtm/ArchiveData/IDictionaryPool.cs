using System.Collections.Generic;
using ArchiveData.DTO;

namespace ArchiveData
{
    public interface IDictionaryPool
    {
        /// <summary>
        /// �������� ������ ����� ������������ ������������ �� �����������
        /// </summary>
        IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; }
    }
}