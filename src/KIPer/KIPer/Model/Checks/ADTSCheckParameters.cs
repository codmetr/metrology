using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTS;

namespace KipTM.Model.Checks
{
    public class ADTSCheckParameters
    {
        private readonly CalibChannel _calibChannel;
        private readonly IDictionary<double, double> _points;
        private readonly IEthalonChannel _ethalonChannel;
        private readonly Func<bool> _getAccept;

        public ADTSCheckParameters(CalibChannel calibChannel, IDictionary<double, double> points, IEthalonChannel ethalonChannel, Func<bool> getAccept)
        {
            _calibChannel = calibChannel;
            _points = points;
            _ethalonChannel = ethalonChannel;
            _getAccept = getAccept;
        }

        public CalibChannel CalibChannel
        {
            get { return _calibChannel; }
        }

        public IDictionary<double, double> Points
        {
            get { return _points; }
        }

        public IEthalonChannel EthalonChannel
        {
            get { return _ethalonChannel; }
        }

        public Func<bool> GetAccept
        {
            get { return _getAccept; }
        }
    }
}
