using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Channels;

namespace KipTM.Model.Checks.Steps
{
    interface ISettedEthalonChannel
    {
        void SetEthalonChannel(IEthalonChannel ehalon);
    }
}
