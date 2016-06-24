using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTSData
{
    public class AdtsTestResults : IAdtsTestResults
    {
        public DateTime CheckTime { get; set; }
        public List<AdtsPointResult> PointsResults { get; set; } 
    }
}
