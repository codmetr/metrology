using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Device;

namespace PACESeries
{
    public class PACEVisaTransport : ITransport
    {
        public PACEVisaTransport(int address)
        {
        }

        public void Send(string command, int address)
        {

        }

        public string Receive(int address)
        {
            return null;
        }
    }
}
