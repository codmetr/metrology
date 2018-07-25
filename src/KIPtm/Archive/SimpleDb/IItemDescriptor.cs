using System.Reflection;

namespace SimpleDb
{
    /// <summary>
    /// Правила получения ключей для полей
    /// </summary>
    public interface IItemDescriptor
    {
        string GetKey(PropertyInfo prop);
    }
}