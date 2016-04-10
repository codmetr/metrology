using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACEVISADriver;

namespace PACE1000VisaTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodGetIDN()
        {
            var pace = new PASE1000("GPIB0::16");
            var idn = pace.GetIdentificator();
            pace.Dispose();
            Assert.IsNotNull(idn);
        }

        [TestMethod]
        public void TestMethodGetPressure()
        {
            var pace = new PASE1000("GPIB0::16");
            var value = pace.GetPressure();
            pace.Dispose();
            Assert.IsFalse(double.IsNaN(value));
        }

        [TestMethod]
        public void TestMethodGetDate()
        {
            var pace = new PASE1000("GPIB0::16");
            var value = pace.GetDate();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodGetPressureUnit()
        {
            var pace = new PASE1000("GPIB0::16");
            var value = pace.GetPressureUnit();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodSetPressureUnit()
        {
            var pace = new PASE1000("GPIB0::16");
            var value = pace.GetPressureUnit();
            var mmGh = pace.SetPressureUnit("MMHG");
            var mbar = pace.SetPressureUnit("MBAR");
            var oldValue = pace.SetPressureUnit(value);
            pace.Dispose();
            Assert.IsTrue(mmGh);
            Assert.IsTrue(mbar);
            Assert.IsTrue(oldValue);
        }

        [TestMethod]
        public void TestMethodGetPressureDefinely()
        {
            var pace = new PASE1000("GPIB0::16");
            var value = pace.GetUnitSpeed();
            pace.Dispose();
            Assert.IsNotNull(value);
        }
    }
}
