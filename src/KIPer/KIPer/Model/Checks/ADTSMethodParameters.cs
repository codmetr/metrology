using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTS;
using KipTM.Model.Channels;
using KipTM.Settings;

namespace KipTM.Model.Checks
{
    public class ADTSMethodParameters
    {
        private readonly CalibChannel _calibChannel;
        private readonly IEnumerable<ADTSPoint> _points;
        private readonly double _rate;
        private readonly PressureUnits _unit;

        public ADTSMethodParameters(CalibChannel calibChannel, IEnumerable<ADTSPoint> points, double rate, PressureUnits unit)
        {
            _calibChannel = calibChannel;
            _points = points;
            _rate = rate;
            _unit = unit;
        }

        public CalibChannel CalibChannel
        {
            get { return _calibChannel; }
        }

        public IEnumerable<ADTSPoint> Points
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
