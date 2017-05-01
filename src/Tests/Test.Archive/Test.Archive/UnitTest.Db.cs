using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDb;

namespace Test.Archive
{
    /// <summary>
    /// Summary description for UnitTest
    /// </summary>
    [TestClass]
    public class UnitTestDb
    {
        public UnitTestDb()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

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
            var tree = TreeParser.Convert(data, new Node() { Name = rootName }, descriptor);

            var db = new DataSource();
            var nodes = NodeLiner.GetNodesFrom(tree);

            db.Clear();
            db.Nodes.AddRange(nodes);
            db.Save();
            db.Load();

            var newTree = NodeLiner.GetNodesFrom(db.Nodes);
            object parsed;
            var resParsing = TreeParser.TryParse(newTree, out parsed, typeof(TestData.CheckSimple), descriptor);
            Assert.IsTrue(resParsing);
            Assert.IsTrue(parsed is TestData.CheckSimple);
            var parsedTyped = parsed as TestData.CheckSimple;
            Assert.IsTrue(parsedTyped != null);
            Assert.AreEqual(parsedTyped.TestResult.Result[0].PointRes, res);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[0].Points, resItems);
            Assert.AreEqual(parsedTyped.TestResult.Result[1].PointRes, res2);
            CollectionAssert.AreEqual(parsedTyped.TestResult.Result[1].Points, resItems2);
        }
    }
}
