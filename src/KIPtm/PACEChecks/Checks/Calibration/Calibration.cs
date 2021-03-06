﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Model.Checks;

namespace PACEChecks.Checks.Calibration
{
    public class Calibration: ICheckMethod
    {

        public string Key { get { return "PaceCalibration"; } }
        public string Title { get { return "Клибровка PACE"; } }

        public object GetCustomConfig(IPropertyPool propertyPool)
        {
            return null;
        }

        public bool Init(object customConf)
        {
            return true;
        }

        public bool Start(CancellationToken cancel)
        {
            return true;
        }

        public IEnumerable<CheckStepConfig> Steps { get; }
    }
}
