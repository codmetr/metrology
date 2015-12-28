using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.Settings
{
    public class ComPortSettings
    {
        public string Name;
        public int NumberCom;
        public int Rate;
        public Parity Parity;
        public int CountBits;
        public int CountStopBits;
    }
}
