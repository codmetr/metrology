using System;
using IEEE488;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACESeries;

namespace PACE1000VisaTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodGetIDN()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
            var idn = pace.GetIdentificator();
            pace.Dispose();
            Assert.IsNotNull(idn);
        }

        [TestMethod]
        public void TestMethodGetPressure()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
            var value = pace.GetPressure();
            pace.Dispose();
            Assert.IsFalse(double.IsNaN(value));
        }

        [TestMethod]
        public void TestMethodGetDate()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
            var value = pace.GetDate();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodGetPressureUnit()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
            var value = pace.GetPressureUnit();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodSetPressureUnit()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
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
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PASE1000Driver(transport);
            var value = pace.GetUnitSpeed();
            pace.Dispose();
            Assert.IsNotNull(value);
        }
    }
}
