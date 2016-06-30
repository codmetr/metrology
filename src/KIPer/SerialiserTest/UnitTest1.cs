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
        public void TestMethod1()
        {
            var result = new TestResult();
            result.Results.Add(new TestStepResult("1", DateTime.Now));
            result.Results.Add(new TestStepResult("2", new AdtsPointResult()));
            var archive = new Archive();
            archive.Save("k", result);
        }
    }
}
