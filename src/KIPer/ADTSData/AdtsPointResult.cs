using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ADTSData
{
    public class AdtsPointResult
    {
        public double Point { get; set; }
        public double Tolerance { get; set; }
        public double RealValue { get; set; }
        public double Error { get; set; }
        public bool IsCorrect { get; set; }
    }
}
