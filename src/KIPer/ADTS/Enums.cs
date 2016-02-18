using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    #region Bool
    public enum Bool
    {
        None = 0,
        OFF = 1,
        ON = 2,
    }
    #endregion

    #region States
    public enum State
    {
        None = 0,
        Control = 2,
        Measure = 3,
        Hold = 4,
    }
    #endregion

    #region Calibration channel

    public enum CalibChannel
    {
        PT,
        PS,
    }
    #endregion

    #region Parameters
    public enum Parameters
    {
        None = 0,
        /// <summary>
        /// Высота (англ. Altitude)
        /// </summary>
        ALT = 1,
        /// <summary>
        /// Калибровочная скорость (англ. Calibratedairspeed)
        /// </summary>
        CAS = 2,
        /// <summary>
        /// Истинная воздушная скорость (англ. Trueairspeed)
        /// </summary>
        TAS = 3,
        /// <summary>
        /// 
        /// </summary>
        MACH = 4,
        /// <summary>
        /// Отношение давления в двигателе (англ. Enginepressureratio)
        /// </summary>
        EPR = 5,
        /// <summary>
        /// Статическое давление (англ. Staticpressure)
        /// </summary>
        PS = 6,
        /// <summary>
        /// Полное (динамическое) давление (англ. Totalpressure (pitot))
        /// </summary>
        PT = 7,
        /// <summary>
        /// Дифференциальное давление (англ. DifferentialpressurePs-Pt)
        /// </summary>
        QC = 8,
    }
    #endregion

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

    #region TRateStates
    public enum TRateStates
    {
        None = 0,
        Off = 1,
        Waiting = 2,
        Timing = 3,
        Timed = 4,
    }
    #endregion

    #region OperationCondition
    [Flags()]
    public enum OperationCondition : byte
    {
        None = 0,
        StableAtAim = 1 << 1,
        StafeAtGround = 1 << 2,
    }
    #endregion
}
