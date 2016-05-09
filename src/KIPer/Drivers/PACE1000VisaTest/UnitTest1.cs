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
            var pace = new PACE1000Driver(transport);
            var idn = pace.GetIdentificator();
            pace.Dispose();
            Assert.IsNotNull(idn);
        }

        [TestMethod]
        public void TestMethodGetPressure()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PACE1000Driver(transport);
            var value = pace.GetPressure();
            pace.Dispose();
            Assert.IsFalse(double.IsNaN(value));
        }

        [TestMethod]
        public void TestMethodGetDate()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PACE1000Driver(transport);
            var value = pace.GetDate();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodGetPressureUnit()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PACE1000Driver(transport);
            var value = pace.GetPressureUnit();
            pace.Dispose();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestMethodSetPressureUnit()
        {
            var transport = new VisaIEEE488();
            transport.Open("GPIB0::16");
            var pace = new PACE1000Driver(transport);
            var value = pace.GetPressureUnit();
            var mmGh = pace.SetPressureUnit(PressureUnits.mmHg);
            var mbar = pace.SetPressureUnit(PressureUnits.MBar);
            var oldValue = value != null && pace.SetPressureUnit(value.Value);
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
            var pace = new PACE1000Driver(transport);
            var value = pace.GetPressureRange();
            pace.Dispose();
            Assert.IsNotNull(value);
        }
    }
}
