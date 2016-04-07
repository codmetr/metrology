using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agilent.TMFramework.Connectivity;
namespace IEEE488Terminal
{
    public class TerminalModel
    {
        /// <summary>
        /// Get a device descriptor
        /// </summary>
        /// <param name="boardID">Board number to which the device is connected</param>
        /// <param name="pad">Primary address of the device</param>
        /// <param name="sad">Secondary address of the device (0 if none)</param>
        /// <param name="tmo">Timeout for the device (Txxx constants: see ibtmo reference in help)</param>
        /// <param name="eot">1 to enable end-of-transmission EOI, 0 to disable EOI</param>
        /// <param name="eos">0 to disable end-of-string termination, nonzero to enable (see ibeos)</param>
        /// <returns>Status outcome</returns>
        public bool TryConnect(int boardID, int pad, int sad, int tmo, int eot, int eos)
        {
            var desc = Ag488Wrap.ibdev(boardID, pad, sad, tmo, eot, eos);
            return desc == 0;
        }


    }
}
