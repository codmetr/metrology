using System;
using ADTSData;
using ArchiveData.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteArchive;

namespace SerialiserTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodSaveAndLoad()
        {
            var result = new TestResult();
            result.Results.Add(new TestStepResult("1", DateTime.Now));
            result.Results.Add(new TestStepResult("2", new AdtsPointResult(){Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2}));
            result.Results.Add(new TestStepResult("3", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            result.Results.Add(new TestStepResult("4", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            result.Results.Add(new TestStepResult("5", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            var archive = new Archive();
            archive.Save("test", result);
            result = archive.Load <TestResult>("test");
            archive.Test();
        }
        [TestMethod]
        public void TestMethodSaveAndLoadSqlite()
        {
            var archive = new Archive();
            archive.Test();
        }
    }
}
