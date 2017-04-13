using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDb;

namespace Test.Archive
{
    [TestClass]
    public class UnitTestTreeParsing
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
            var tree = TreeParser.Convert(data, new Node() { Name = rootName}, descriptor);

            var resNode = tree["TestResult"]["Result"]["PointRes"].Value;
            Assert.AreEqual(resNode, res);

            object parsed;
            var resParsing = TreeParser.TryParse(tree, out parsed, typeof (CheckSimple), descriptor);
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
}
