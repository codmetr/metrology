using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public interface IEthalonChannel
    {
        double GetEthalonValue(double point, CancellationToken calcel);
    }
}
