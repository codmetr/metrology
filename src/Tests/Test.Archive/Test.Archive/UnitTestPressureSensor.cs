using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PressureSensorData;
using SQLiteArchive;

namespace Test.Archive
{
    /// <summary>
    /// Summary description for UnitTestPressureSensor
    /// </summary>
    [TestClass]
    public class UnitTestPressureSensor
    {
        public UnitTestPressureSensor()
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
        public void TestSave()
        {
            DataAccessor accessor = new DataAccessor(UtTools.GetDataPool(TestContext));

            TestResultID id1;
            PressureSensorResult result1;
            PressureSensorConfig config1;
            UtTools.FillTestData(out id1, out result1, out config1);
            accessor.Add(id1, result1, config1);

            TestResultID id2;
            PressureSensorResult result2;
            PressureSensorConfig config2;
            UtTools.FillTestData(out id2, out result2, out config2);
            accessor.Add(id2, result2, config2);

            Assert.AreNotEqual(id1, id2);
        }
        [TestMethod]
        public void TestSaveLoadCompare()
        {
            DataAccessor accessor = new DataAccessor(UtTools.GetDataPool(TestContext));

            TestResultID id1;
            PressureSensorResult result1;
            PressureSensorConfig config1;
            UtTools.FillTestData(out id1, out result1, out config1);
            accessor.Add(id1, result1, config1);

            TestResultID id2;
            PressureSensorResult result2;
            PressureSensorConfig config2;
            UtTools.FillTestData(out id2, out result2, out config2);
            accessor.Add(id2, result2, config2);

            Assert.IsFalse(UtTools.CompareData(id1, id2));

            var dataPool = UtTools.GetDataPool(TestContext);
            accessor = new DataAccessor(dataPool);
            var res1Loaded = (PressureSensorResult)accessor.Load(id1);
            var conf1Loaded = (PressureSensorConfig)accessor.LoadConfig(id1);
            Assert.IsTrue(UtTools.CompareData(result1, res1Loaded));
            Assert.IsTrue(UtTools.CompareData(config1, conf1Loaded));
            var res2Loaded = (PressureSensorResult)accessor.Load(id2);
            var conf2Loaded = (PressureSensorConfig)accessor.LoadConfig(id2);
            Assert.IsTrue(UtTools.CompareData(result2, res2Loaded));
            Assert.IsTrue(UtTools.CompareData(config2, conf2Loaded));
        }
    }
}
