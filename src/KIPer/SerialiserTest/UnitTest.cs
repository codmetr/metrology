using System;
using System.Collections.Generic;
using ADTSData;
using ArchiveData.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteArchive;
using System.IO;

namespace SerialiserTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodSaveAndLoad()
        {
            var result = new TestResult();
            result.Results.Add(new TestStepResult("", "", "1", DateTime.Now));
            result.Results.Add(new TestStepResult("", "", "2", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            result.Results.Add(new TestStepResult("", "", "3", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            result.Results.Add(new TestStepResult("", "", "4", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            result.Results.Add(new TestStepResult("", "", "5", new AdtsPointResult() { Error = 0.1, IsCorrect = true, Point = 100, RealValue = 100.1, Tolerance = 0.2 }));
            var archive = new Archive();
            archive.Save("test", result);
            result = archive.Load <TestResult>("test");
        }
        [TestMethod]
        public void TestMethodSaveAndLoadSqlite()
        {
            var archive = new Archive();
            archive.Test();
        }

        [TestMethod]
        public void TestListOfList()
        {
            var result = new List<List<string>>()
            {
                new List<string>(){"1", "2"},
                new List<string>(){"3", "4"},
            };
            var archive = new Archive();
            archive.Save("test", result);
            result = archive.Load<List<List<string>>>("test");
        }

        [TestMethod]
        public void TestLoad()
        {
            var repo = new SqliteRepo();
            repo.CreateRepository("123.db");
        }
    }
}
