using System;
using System.Collections.Generic;
using System.Text;
using Ivi.Visa.Interop;

namespace VisaDriver
{
    public class Visa:IDisposable
    {
        private Ivi.Visa.Interop.ResourceManager rm;
        private Ivi.Visa.Interop.FormattedIO488 ioArbFG;
        private Ivi.Visa.Interop.IMessage msg;

        public Visa(string address)
        {
            try
            {
                rm = new ResourceManager();
                ioArbFG = new FormattedIO488Class();
                this.msg = (rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                this.ioArbFG.IO = msg;
            }
            catch (SystemException ex)
            {
                this.ioArbFG.IO = null;
                throw;
            }
        }

        public bool SetAddress(string address)
        {
            if (this.ioArbFG.IO!=null)
                this.ioArbFG.IO.Close();
            try
            {
                rm = new ResourceManager();
                ioArbFG = new FormattedIO488Class();
                this.msg = (rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                this.ioArbFG.IO = msg;
                return true;
            }
            catch (SystemException ex)
            {
                this.ioArbFG.IO = null;
                return false;
            }
        }

        public void WriteString(string message)
        {
            ioArbFG.WriteString(message, true);
        }

        public string ReadString()
        {
            return ioArbFG.ReadString();
        }

        public void Dispose()
        {
            ioArbFG.IO.Close();
        }
    }
}
