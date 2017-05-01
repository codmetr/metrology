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
            var data = TestData.GetTestData(res, resItems, res2, resItems2);

            var descriptor = new ItemDescriptor();
            var rootName = "root";
            var tree = TreeParser.Convert(data, new Node() { Name = rootName}, descriptor);

            var resNode = tree["TestResult"]["Result"]["PointRes"].Val;
            Assert.AreEqual(resNode, res);

            object parsed;
            var resParsing = TreeParser.TryParse(tree, out parsed, typeof (TestData.CheckSimple), descriptor);
            Assert.IsTrue(resParsing);
            Assert.IsTrue(parsed is TestData.CheckSimple);
            var parsedTyped = parsed as TestData.CheckSimple;
            Assert.IsTrue(parsedTyped !=null);
            Assert.AreEqual(parsedTyped.TestResult.Result[0].PointRes, res);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[0].Points, resItems);
            Assert.AreEqual(parsedTyped.TestResult.Result[1].PointRes, res2);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[1].Points, resItems2);
        }
    }

}
