﻿using System;
using Ivi.Visa.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Agilent.TMFramework.Connectivity;

namespace IEEE488UnitTest
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var rm = new ResourceManager();
            //FormattedIO488 ioArbFG = new FormattedIO488Class();
            IMessage msg;
            try
            {
                msg = (rm.Open("GPIB0::16", Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                //ioArbFG.IO = msg;
            }
            catch (SystemException ex)
            {
                ex.ToString();
               //ioArbFG.IO = null;
                return;
            }

            //string m_strReturn;
            //ioArbFG = new FormattedIO488Class();
            //ioArbFG.WriteString("*IDN?", true);
            //m_strReturn = ioArbFG.ReadString();
        }
    }
}
