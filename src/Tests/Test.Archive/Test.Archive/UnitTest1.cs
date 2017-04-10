using System;
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
            var data = new CheckSimple() {TestResult = new TestSimple() {Result = new ResultSimple() {PointRes = res } }};

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
            Assert.AreEqual(parsedTyped.TestResult.Result.PointRes, res);
        }
    }

    /// <summary>
    /// Тестовый класс - корень/сессия/проверка
    /// </summary>
    public class CheckSimple
    {
        public TestSimple TestResult { get; set; } = new TestSimple();
    }

    /// <summary>
    /// Тестовый класс - тест/тех.карта
    /// </summary>
    public class TestSimple
    {
        public ResultSimple Result { get; set; } = new ResultSimple();
    }

    /// <summary>
    /// Тестовый класс - результат
    /// </summary>
    public class ResultSimple
    {
        public int PointRes { get; set; } = 777;
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
                {
                    node.Childs.Add(new Node() { Name = descriptor.GetKey(property), Value = propValue, Parrent = node});
                    return node;
                }

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
                    {
                        property.SetValue(res, item.Value, null);
                        continue;
                    }

                    object itemTarget;

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
    }

    [DebuggerDisplay("{Name}:{Value}")]
    public class Node
    {
        private static int _maxId = 0;

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

        public List<Node> Childs { get; set; } = new List<Node>();

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
