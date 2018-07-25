using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
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
        PTPS,
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

    [Flags]
    public enum Status
    {
        /// <summary>
        /// Set after 15 seconds (to give pressure time to settle)
        /// after both bits 10 and 8 (if not in Pt only mode) become set.
        /// </summary>
        /// <remarks>
        /// Bit 1 should generally be used to indicate that all pressures are stable before taking readings,
        /// rather than bits 8 or 10 as bit 1 includes stabilization time.
        /// </remarks>
        StableAtAimValue = 1<<1,
        SafeAtGround = 1<<2,
        /// <summary>
        /// This is set to 1 if bit 9 is set and bit 11 is set.
        /// </summary>
        Ramping = 1<<3,
        /// <summary>
        /// Set immediately the aim set-point is reached.
        /// </summary>
        PsAtSetPointAndInControlMode = 1<<8,
        PsRampingAndAchievingRate = 1<<9,
        /// <summary>
        /// Set immediately the aim set-point is reached.
        /// </summary>
        PtAtSetPointAndInControlMode = 1<<10,
        PtRampingAndAchievingRate = 1<<11,
    }
}
