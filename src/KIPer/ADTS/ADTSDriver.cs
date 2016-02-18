using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSDriver
    {
        private int _address;
        private ADTSParser _parser;


        public bool CalibrationAbort(IEEE488.ITransportIEEE488 transport)
        {
            var cmd = _parser.GetCommandCalibrationAbort();
            return transport.Send(_address, cmd);
        }
    }
}
