using System;
using System.Collections.Generic;
using System.Text;
using Ivi.Visa.Interop;

namespace VISAConsole
{
    class Program
    {
        private static FormattedIO488 ioArbFG;
        static void Main(string[] args)
        {
            var rm = new ResourceManager();
            ioArbFG = new FormattedIO488Class();
            IMessage msg;
            try
            {
                msg = (rm.Open("GPIB0::16", Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                ioArbFG.IO = msg;
                string m_strReturn;
                ioArbFG.WriteString("*IDN?", true);
                m_strReturn = ioArbFG.ReadString();
            }
            catch (SystemException ex)
            {
                ex.ToString();
                //ioArbFG.IO = null;
                return;
            }

            //string m_strReturn;
            //ioArbFG = new FormattedIO488Class();
            //ioArbFG.WriteString("*IDN?", true);
            //m_strReturn = ioArbFG.ReadString();
        }
    }
}
