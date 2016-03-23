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
    public class ADTSCheckParameters
    {
        private readonly CalibChannel _calibChannel;
        private readonly IEnumerable<ADTSChechPoint> _points;

        public ADTSCheckParameters(CalibChannel calibChannel, IEnumerable<ADTSChechPoint> points)
        {
            _calibChannel = calibChannel;
            _points = points;
        }

        public CalibChannel CalibChannel
        {
            get { return _calibChannel; }
        }

        public IEnumerable<ADTSChechPoint> Points
        {
            get { return _points; }
        }
    }
}
