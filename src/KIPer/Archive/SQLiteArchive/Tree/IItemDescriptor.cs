using System.Reflection;

namespace SQLiteArchive.Tree
{
    /// <summary>
    /// Правила получения ключей для полей
    /// </summary>
    public interface IItemDescriptor
    {
        string GetKey(PropertyInfo prop);
    }
}