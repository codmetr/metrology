using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using VisaDriver;

namespace IEEE488
{
    public class VisaIEEE488 : ITransportIEEE488
    {
        private readonly VisaDriver.Visa _visa;

        public VisaIEEE488(Visa visa)
        {
            _visa = visa;
        }

        public bool Open(int address)
        {
            return _visa.SetAddress(string.Format("GPIB0::{0}", address));
        }

        public bool Close(int address)
        {
            _visa.Dispose();
            return true;
        }

        public bool Send(int address, string data)
        {
            _visa.WriteString(data);
            return true;
        }

        public string Receive(int address)
        {
            return _visa.ReadString();
        }
    }
}
