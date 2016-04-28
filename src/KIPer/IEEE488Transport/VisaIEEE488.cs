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
        private VisaDriver.Visa _visa;
        
        public VisaIEEE488()
        {
            _visa = null;
        }

        public bool Open(string address)
        {
            if (_visa == null)
            {
                _visa = new Visa(address);
                return true;
            }
            return _visa.SetAddress(address);
        }

        public bool Open(int address)
        {
            return Open(string.Format("GPIB0::{0}", address));
        }

        public bool Close()
        {
            if (_visa != null)
                _visa.Dispose();
            _visa = null;
            return true;
        }

        public bool Close(int address)
        {
            return Close();
        }

        public bool Send(string data)
        {
            if (_visa == null)
                throw new Exception("Call Send before \"Open\"");
            _visa.WriteString(data);
            return true;
        }

        public string Receive()
        {
            if (_visa == null)
                throw new Exception("Call Receive before \"Open\"");
            return _visa.ReadString();
        }
    }
}
