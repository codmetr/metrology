using System;
using System.Linq;
using System.Reflection;

namespace SQLiteArchive.Tree
{
    /// <summary>
    /// Правила получения ключей для полей:
    /// Если есть атрибут KeyAttribute - по нему,
    /// если нет - по имени свойства
    /// </summary>
    public class ItemDescriptor : IItemDescriptor
    {
        public class KeyAttribute : Attribute
        {
            public KeyAttribute(string key)
            {
                Key = key;
            }

            public string Key { get; private set; }
        }


        public string GetKey(PropertyInfo prop)
        {
            var atr = prop.GetCustomAttributes(typeof(KeyAttribute), false).OfType<KeyAttribute>();
            if (atr.Any())
                return atr.First().Key;
            return prop.Name;
        }
    }
}