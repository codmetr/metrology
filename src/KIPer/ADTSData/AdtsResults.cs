using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTSData
{
    public class AdtsTestResults : IAdtsTestResults
    {
        private List<AdtsPointResult> _pointsResults = new List<AdtsPointResult>();
        public DateTime CheckTime { get; set; }

        public List<AdtsPointResult> PointsResults
        {
            get { return _pointsResults; }
            set { _pointsResults = value; }
        }
    }
}
