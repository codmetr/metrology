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
        }

        public bool Open(int address)
        {
            if (_visa == null)
            {
                _visa = new Visa(string.Format("GPIB0::{0}", address));
                return true;
            }
            return _visa.SetAddress(string.Format("GPIB0::{0}", address));
        }

        public bool Close(int address)
        {
            if (_visa != null)
                _visa.Dispose();
            _visa = null;
            return true;
        }

        public bool Send(int address, string data)
        {
            if (_visa == null)
                throw new Exception("Call Send after \"Open\"");
            _visa.WriteString(data);
            return true;
        }

        public string Receive(int address)
        {
            if (_visa == null)
                throw new Exception("Call Receive after \"Open\"");
            return _visa.ReadString();
        }
    }
}
