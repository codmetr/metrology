using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Archive
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var res = 555;
            var res2 = 132;
            var resItems = new List<int>() { 5, 6, 7 };
            var resItems2 = new List<int>() { 8, 9, 10 };
            var resData = new List<ResultSimple>() {
                new ResultSimple() { PointRes = res, Points = resItems,},
                new ResultSimple() { PointRes = res2, Points = resItems2,}};
            var data = new CheckSimple() {TestResult = new TestSimple() {Result = resData } };

            var descriptor = new ItemDescriptor();
            var rootName = "root";
            var tree = Parser.Convert(data, new Node() { Name = rootName}, descriptor);

            var resNode = tree["TestResult"]["Result"]["PointRes"].Value;
            Assert.AreEqual(resNode, res);

            object parsed;
            var resParsing = Parser.TryParse(tree, out parsed, typeof (CheckSimple), descriptor);
            Assert.IsTrue(resParsing);
            Assert.IsTrue(parsed is CheckSimple);
            var parsedTyped = parsed as CheckSimple;
            Assert.IsTrue(parsedTyped !=null);
            Assert.AreEqual(parsedTyped.TestResult.Result[0].PointRes, res);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[0].Points, resItems);
            Assert.AreEqual(parsedTyped.TestResult.Result[1].PointRes, res2);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[1].Points, resItems2);
        }
    }

    /// <summary>
    /// Тестовый класс - корень/сессия/проверка
    /// </summary>
    public class CheckSimple
    {
        private TestSimple _testResult = new TestSimple();

        public TestSimple TestResult
        {
            get { return _testResult; }
            set { _testResult = value; }
        }
    }

    /// <summary>
    /// Тестовый класс - тест/тех.карта
    /// </summary>
    public class TestSimple
    {
        private List<ResultSimple> _result = new List<ResultSimple>();

        public List<ResultSimple> Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }

    /// <summary>
    /// Тестовый класс - результат
    /// </summary>
    public class ResultSimple
    {
        private int _pointRes = 777;
        private List<int> _points = new List<int> { 1, 2, 3 };

        public int PointRes
        {
            get { return _pointRes; }
            set { _pointRes = value; }
        }

        public List<int> Points
        {
            get { return _points; }
            set { _points = value; }
        }
    }

    /// <summary>
    /// Парсер
    /// </summary>
    public class Parser
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

            var properties = itemType.GetProperties().Where(p=>p.CanRead&&p.CanWrite);

            foreach (var property in properties)
            {
                var propValue = property.GetValue(item, null);
                if (IsSimple(property.PropertyType))
                { // добавление элемента простого типа
                    node.Childs.Add(new Node() { Name = descriptor.GetKey(property), Value = propValue, Parrent = node});
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
                            var itemNode = new Node() {Name = descriptor.GetKey(property), Parrent = node};
                            Convert(subItem, itemNode, descriptor);
                            node.Childs.Add(itemNode);
                        }

                    }
                    continue;
                }

                // добавление элемента сложного типа
                var newNode = new Node() { Name = descriptor.GetKey(property), Parrent = node};
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

                        var listType = typeof (List<>).MakeGenericType(typeItem);
                        var paramListValue = (IList)Activator.CreateInstance(listType);
                        foreach (var propNode in propNodes)
                        {
                            if(IsSimple(typeItem))
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
            return type == typeof (int);
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

    ;

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
                if(value == node)
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
        public class KeyAttribute:Attribute
        {
            public KeyAttribute(string key)
            {
                Key = key;
            }

            public string Key { get; private set; }
        }


        public string GetKey(PropertyInfo prop)
        {
            var atr = prop.GetCustomAttributes(typeof (KeyAttribute), false).OfType<KeyAttribute>();
            if (atr.Any())
                return atr.First().Key;
            return prop.Name;
        }
    }
}
