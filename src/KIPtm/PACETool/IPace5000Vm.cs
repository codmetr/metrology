using System;
using PACESeries;

namespace PACETool
{
    internal interface IPace5000Vm
    {
        void SetPressure(double pressure);
        void SetUnit(PressureUnits unit);
        void SetPaceMode(bool isControll);
        void SetAimVal(double aim);
        event Action<TimeSpan> CallStartAutoUpdate;
        event Action CallStopAutoUpdate;
        event Action CallUpdatePressureAndUnits;
        event Action CallUpdateUnits;
        event Action<PressureUnits> CallSetSelectedUnit;
        event Action CallSetLloOn;
        event Action CallSetLloOff;
        event Action CallSetLocal;
        event Action CallSetRemote;
        event Action CallToControll;
        event Action CallToMeasuring;
        event Action<double> CallSetAim;
        event Action CallReadAim;
    }
}