using System.Reflection;

namespace SQLiteArchive.Tree
{
    /// <summary>
    /// ������� ��������� ������ ��� �����
    /// </summary>
    public interface IItemDescriptor
    {
        string GetKey(PropertyInfo prop);
    }
}