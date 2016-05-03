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
        MBar = 1,
        inH2O4 = 2,
        inH2O20 = 3,
        inHg = 4,
        mmHg = 5,
        Pa = 6,
        hPa = 7,
        psi = 8,
        inH2O60F = 9,
        KgCm2 = 10,
        FS = 11,
        mmH2O4 = 12,
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
