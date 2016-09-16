using System.Collections.Generic;
using ADTS;

namespace ADTSChecks.Model.Checks
{
    public class ADTSMethodParameters
    {
        private readonly CalibChannel _calibChannel;
        private readonly List<ADTSPoint> _points;
        private readonly double _rate;
        private readonly PressureUnits _unit;

        public ADTSMethodParameters(CalibChannel calibChannel, IEnumerable<ADTSPoint> points, double rate, PressureUnits unit)
        {
            _calibChannel = calibChannel;
            _points = new List<ADTSPoint>(points);
            _rate = rate;
            _unit = unit;
        }

        public CalibChannel CalibChannel
        {
            get { return _calibChannel; }
        }

        public List<ADTSPoint> Points
        {
            get { return _points; }
        }

        public double Rate
        {
            get { return _rate; }
        }

        public PressureUnits Unit
        {
            get { return _unit; }
        }

    }
}
