using System.Reflection;

namespace SimpleDb
{
    /// <summary>
    /// ������� ��������� ������ ��� �����
    /// </summary>
    public interface IItemDescriptor
    {
        string GetKey(PropertyInfo prop);
    }
}