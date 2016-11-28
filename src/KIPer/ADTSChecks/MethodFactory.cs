﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using KipTM.Interfaces;
using KipTM.Model.Checks;

namespace ADTSChecks
{
    public class MethodFactory : IMethodFactory
    {
        public Tuple<string, Dictionary<string, ICheckMethod>> GetDefault()
        {
            var adtsCheck = new Calibration(NLog.LogManager.GetLogger("ADTSCheckMethod"));
            var adtsTest = new Test(NLog.LogManager.GetLogger("ADTSTestMethod"));
            return new Tuple<string, Dictionary<string, ICheckMethod>>(ADTSModel.Key,new Dictionary<string, ICheckMethod>()
            {
                {Test.Key, adtsTest},
                {Calibration.Key, adtsCheck},
            }); 

        }
    }
}
