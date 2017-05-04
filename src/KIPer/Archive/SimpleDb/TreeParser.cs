using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace SimpleDb
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
                node.Val = item;
                return node;
            }

            var properties = itemType.GetProperties().Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                var propValue = property.GetValue(item, null);
                if (IsSimple(property.PropertyType))
                { // добавление элемента простого типа
                    node.Childs.Add(new Node()
                    {
                        Name = descriptor.GetKey(property),
                        Val = propValue,
                        Parrent = node,
                        TypeVal = (int) GetTypeValue(property.PropertyType),
                    });
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
                                Val = subItem,
                                Parrent = node,
                                TypeVal = (int)GetTypeValue(typeItem),
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
                        property.SetValue(res, item.Val, null);
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
                                paramListValue.Add(propNode.Val); //наполнение коллекции простых типов
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
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

        public static readonly Dictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            //[typeof(byte[])] = DbType.Binary,
            //[typeof(byte?)] = DbType.Byte,
            //[typeof(sbyte?)] = DbType.SByte,
            //[typeof(short?)] = DbType.Int16,
            //[typeof(ushort?)] = DbType.UInt16,
            //[typeof(int?)] = DbType.Int32,
            //[typeof(uint?)] = DbType.UInt32,
            //[typeof(long?)] = DbType.Int64,
            //[typeof(ulong?)] = DbType.UInt64,
            //[typeof(float?)] = DbType.Single,
            //[typeof(double?)] = DbType.Double,
            //[typeof(decimal?)] = DbType.Decimal,
            //[typeof(bool?)] = DbType.Boolean,
            //[typeof(char?)] = DbType.StringFixedLength,
            //[typeof(Guid?)] = DbType.Guid,
            //[typeof(DateTime?)] = DbType.DateTime,
            //[typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            //[typeof(TimeSpan?)] = DbType.Time,
            //[typeof(object)] = DbType.Object
        };

        private static DbType GetTypeValue(Type val)
        {
            if(!TypeMap.ContainsKey(val))
                throw new KeyNotFoundException(string.Format("For type {0} not founf DbType", val));
            return TypeMap[val];
        }

        public static object ParceValue(string val, int typeVal)
        {
            var t = (DbType) typeVal;
            object res = val;

            switch (t)
            {
                case DbType.Byte:
                    res = byte.Parse(val);
                    break;
                case DbType.Boolean:
                    res = bool.Parse(val);
                    break;
                case DbType.DateTime:
                    res = DateTime.Parse(val);
                    break;
                case DbType.Decimal:
                    res = decimal.Parse(val);
                    break;
                case DbType.Double:
                    res = double.Parse(val);
                    break;
                case DbType.Guid:
                    res = Guid.Parse(val);
                    break;
                case DbType.Int16:
                    res = short.Parse(val);
                    break;
                case DbType.Int32:
                    res = int.Parse(val);
                    break;
                case DbType.Int64:
                    res = long.Parse(val);
                    break;
                case DbType.SByte:
                    res = sbyte.Parse(val);
                    break;
                case DbType.Single:
                    res = float.Parse(val);
                    break;
                case DbType.String:
                    break;
                case DbType.Time:
                    res = TimeSpan.Parse(val);
                    break;
                case DbType.UInt16:
                    res = ushort.Parse(val);
                    break;
                case DbType.UInt32:
                    res = uint.Parse(val);
                    break;
                case DbType.UInt64:
                    res = ulong.Parse(val);
                    break;
                case DbType.StringFixedLength:
                    res = val[0];
                    break;
                case DbType.DateTimeOffset:
                    res = DateTimeOffset.Parse(val);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return res;
        }
    }
}