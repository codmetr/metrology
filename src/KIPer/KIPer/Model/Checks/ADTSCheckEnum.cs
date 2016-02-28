using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public enum ADTSCheckError
    {
        ErrorStartCalibration,
        ErrorSetPressurePoint,
        ErrorSetRealValue,
        ErrorGetResultCalibration,
        ErrorAcceptResultCalibration
    }
}
