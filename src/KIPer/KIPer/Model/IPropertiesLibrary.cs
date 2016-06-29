using KipTM.Archive;

namespace KipTM.Model
{
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