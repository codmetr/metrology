using KipTM.Archive;

namespace KipTM.Model
{
    /// <summary>
    /// ���� �������
    /// </summary>
    public interface IPropertiesLibrary
    {
        /// <summary>
        /// ������ �������� ���� ��������
        /// </summary>
        DataPool PropertyPool { get; }

        /// <summary>
        /// ������ �������� (����� �������������, ����������� � ��.)
        /// </summary>
        DictionariesPool DictionariesPool { get; }
    }
}