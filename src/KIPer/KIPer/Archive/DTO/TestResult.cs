﻿using System;
using System.Collections.Generic;
using KipTM.Model.Devices;

namespace KipTM.Archive.DTO
{
    public class TestResult
    {
        public string User { get; set; }

        public string Note { get; set; }

        public string CheckType { get; set; }

        public string Channel { get; set; }

        public DateTime Timestamp { get; set; }

        public DeviceDescriptor TargetDevice { get; set; }

        public List<DeviceDescriptor> Etalon { get; set; }

        public List<PointResult> Parameters { get; set; } 
    }
}
