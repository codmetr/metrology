using System;
using System.Collections.Generic;
using System.Text;

namespace PACESeries
{
    enum PeremeterTypes
    {
        Real,
        Integer,
        Boolean,
        String,
        State,
        PressureUnit,
    }
        
    #region PressureUnits
    public enum PressureUnits
    {
        None = 0,
        MBar,
        Bar,
        inH2O4,
        inH2O,
        inHg,
        mmHg,
        Pa,
        hPa,
        kPa,
        psi,
        inH2O60F,
        KgCm2,
        ATM,
        mmH2O4,
    }

    public enum AeronauticalUnits
    {
        None = 0,
        FTKNTS = 1,
        MKPH = 2,
        MKPH_Mmin = 3,
        MKPH_hMmin = 4,
        MKPH_Ms = 5,
    }

    public enum TemperatureUnits
    {
        None = 0,
        C = 1,
        F = 2,
    }
    #endregion

}
