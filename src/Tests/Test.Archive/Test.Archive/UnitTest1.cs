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
            var data = new CheckSimple();

            var tree = Parser.Convert(data, new Node());

            Assert.AreEqual(tree.Id, 0);
        }
    }

    public class CheckSimple
    {
        public TestSimple TestResult { get; set; } = new TestSimple();
    }

    public class TestSimple
    {
        public ResultSimple Result { get; set; } = new ResultSimple();
    }

    public class ResultSimple
    {
        public int PointRes { get; set; } = 777;
    }

    public class Parser
    {
        public static Node Convert(object item, Node root)
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
                    node.Childs.Add(new Node() { Name = property.Name, Value = propValue, Parrent = node});
                    return node;
                }

                var newNode = new Node() { Name = property.Name, Parrent = node};
                Convert(propValue, newNode);
                node.Childs.Add(newNode);
            }

            return node;
        }

        public static bool IsSimple(Type type)
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
    }
}
