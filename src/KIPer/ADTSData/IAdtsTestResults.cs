using System;
using System.Collections.Generic;

namespace ADTSData
{
    public interface IAdtsTestResults
    {
        DateTime CheckTime { get; set; }
        List<AdtsPointResult> PointsResults { get; set; }
    }
}