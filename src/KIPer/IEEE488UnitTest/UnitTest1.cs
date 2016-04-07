using System;
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
            int boardID=0;
            int pad=16;
            int sad=0;
            int tmo=500;
            int eot=1;
            int eos=0;
            var desc = Ag488Wrap.ibdev(boardID, pad, sad, tmo, eot, eos);
            desc.ToString();
        }
    }
}
