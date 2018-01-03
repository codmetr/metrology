using System;
using System.Text;
using System.Collections.Generic;
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
        public void TestMethod1()
        {
            TestResultID id;
            PressureSensorResult result;
            PressureSensorConfig config;
            DataAccessorSqLite accessor = GetAccessor();
            FillTestData(out id, out result, out config);
            accessor.Add(id, result, config);

        }

        private DataAccessorSqLite GetAccessor()
        {
            var testDb = "test.db";
            Console.WriteLine($"TestResultsDirectory: {TestContext.TestResultsDirectory}");
            var ds = new DataSource(testDb, Console.WriteLine);
            var listDevDescriptors = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor("devModel", "devCommonType", "manufacturer")
            };
            var dictMoq = new Mock<IDictionaryPool>();
            dictMoq.Setup(foo => foo.DeviceTypes).Returns(listDevDescriptors);
            var resTypes = new Dictionary<string, Type>() { {"keyType", typeof(PressureSensorResult) } };
            var confTypes = new Dictionary<string, Type>() { { "keyType", typeof(PressureSensorConfig) } };
            var accessor = new DataAccessorSqLite(DataPool.Load(dictMoq.Object, resTypes, confTypes, Path.Combine(TestContext.TestResultsDirectory, testDb), Console.WriteLine));
            return accessor;
        }

        private void FillTestData(out TestResultID id, out PressureSensorResult result, out PressureSensorConfig config)
        {
            id = new TestResultID()
            {
                TargetDeviceKey = "keyType",
                DeviceType = "devType",
            };
            result = new PressureSensorResult();
            config = new PressureSensorConfig();
        }
    }
}
