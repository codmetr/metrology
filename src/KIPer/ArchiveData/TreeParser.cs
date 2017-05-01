using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArchiveData
{

    /// <summary>
    /// Парсер
    /// </summary>
    public class TreeParser
    {
        /// <summary>
        /// Преобразовать в дерево
        /// </summary>
        /// <param name="item"></param>
        /// <param name="root"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static Node Convert(object item, Node root, IItemDescriptor descriptor)
        {
            Node node = root;

            var itemType = item.GetType();

            if (IsSimple(itemType))
            {
                node.Value = item;
                return node;
            }

            var properties = itemType.GetProperties().Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                var propValue = property.GetValue(item, null);
                if (IsSimple(property.PropertyType))
                { // добавление элемента простого типа
                    node.Childs.Add(new Node() { Name = descriptor.GetKey(property), Value = propValue, Parrent = node });
                    continue;
                }

                if (IsList(property.PropertyType))
                { // добавление коллекции
                    IList propItems = propValue as IList;
                    var typeItem = property.PropertyType.GetGenericArguments()[0];
                    foreach (var subItem in propItems)
                    {
                        if (IsSimple(typeItem))
                        { // добавление элемента коллекции сложного типа
                            node.Childs.Add(new Node()
                            {
                                Name = descriptor.GetKey(property),
                                Value = subItem,
                                Parrent = node
                            });
                        }
                        else
                        { // добавление элемента коллекции сложного типа
                            var itemNode = new Node() { Name = descriptor.GetKey(property), Parrent = node };
                            Convert(subItem, itemNode, descriptor);
                            node.Childs.Add(itemNode);
                        }
                    }
                    continue;
                }

                // добавление элемента сложного типа
                var newNode = new Node() { Name = descriptor.GetKey(property), Parrent = node };
                Convert(propValue, newNode, descriptor);
                node.Childs.Add(newNode);
            }

            return node;
        }

        /// <summary>
        /// Попробовать разобрать
        /// </summary>
        /// <param name="root"></param>
        /// <param name="res"></param>
        /// <param name="targetType"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static bool TryParse(Node root, out object res, Type targetType, IItemDescriptor descriptor)
        {
            try
            {
                res = targetType.Assembly.CreateInstance(targetType.FullName);
                var properties = targetType.GetProperties().Where(p => p.CanRead && p.CanWrite);
                foreach (var property in properties)
                {
                    var key = descriptor.GetKey(property);
                    var item = root.Childs.FirstOrDefault(node => node.Name == key);
                    if (item == null)
                        continue;
                    if (IsSimple(property.PropertyType))
                    {// разбор простых типов
                        property.SetValue(res, item.Value, null);
                        continue;
                    }

                    if (IsList(property.PropertyType))
                    {// разбор коллекции
                        var propNodes = root.Childs.Where(node => node.Name == key);

                        var typeItem = property.PropertyType.GetGenericArguments()[0];

                        var listType = typeof(List<>).MakeGenericType(typeItem);
                        var paramListValue = (IList)Activator.CreateInstance(listType);
                        foreach (var propNode in propNodes)
                        {
                            if (IsSimple(typeItem))
                                paramListValue.Add(propNode.Value); //наполнение коллекции простых типов
                            else
                            {
                                object itemElement;
                                if (!TryParse(propNode, out itemElement, typeItem, descriptor))
                                    continue;
                                paramListValue.Add(itemElement); //наполнение коллекции сложных типов
                            }
                        }
                        property.SetValue(res, paramListValue, null);
                        continue;
                    }

                    object itemTarget; // разбор сложного типа
                    if (!TryParse(item, out itemTarget, property.PropertyType, descriptor))
                        continue;

                    property.SetValue(res, itemTarget, null);
                }
            }
            catch
            {
                res = null;
                return false;
            }

            return true;
        }


        private static bool IsSimple(Type type)
        {
            return type == typeof(int);
        }

        private static bool IsList(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }
    }

    [DebuggerDisplay("{Name}:{Value}")]
    public class Node
    {
        private static int _maxId = 0;
        private List<Node> _childs = new List<Node>();

        public static int GetNewId()
        {
            return _maxId++;
        }

        public Node()
        {
            Id = GetNewId();
        }


        public Node(int id)
        {
            Id = id;
        }

        public string Name { get; set; }

        public int Id { get; set; }

        public Node Parrent { get; set; }

        public List<Node> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public object Value { get; set; }

        public Node this[string key]
        {
            get
            {
                var res = Childs.FirstOrDefault(item => item.Name == key);
                return res;
            }
            set
            {
                var node = Childs.FirstOrDefault(item => item.Name == key);
                if (value == node)
                    return;
                if (node != null)
                    Childs.Remove(node);
                Childs.Add(value);
            }
        }
    }

    /// <summary>
    /// Правила получения ключей для полей
    /// </summary>
    public interface IItemDescriptor
    {
        string GetKey(PropertyInfo prop);
    }

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
