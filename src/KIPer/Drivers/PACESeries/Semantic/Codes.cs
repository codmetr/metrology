using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries.Semantic
{
    public enum Codes
    {
        [CodeDescriptorAttribue("ABOR", "Abort")]
        ABOR,
        [CodeDescriptorAttribue("ADDR", "Address")]
        ADDR,
        [CodeDescriptorAttribue("ALT", "Altitude")]
        ALT,
        [CodeDescriptorAttribue("AMPL", "Amplitude")]
        AMPL,
        [CodeDescriptorAttribue("ATOD", "Analog to Digital")]
        ATOD,
        [CodeDescriptorAttribue("AVER", "Average")]
        AVER,
        [CodeDescriptorAttribue("BAR", "Barometer")]
        BAR,
        [CodeDescriptorAttribue("BRID", "Bridge")]
        BRID,
        [CodeDescriptorAttribue("CAL", "Calibration")]
        CAL,
        [CodeDescriptorAttribue("CAT", "Catalogue")]
        CAT,
        [CodeDescriptorAttribue("CDIS", "Cdisable (calibration disable)")]
        CDIS,
        [CodeDescriptorAttribue("CEN", "Cenable (calibration enable)")]
        CEN,
        [CodeDescriptorAttribue("CLS", "Clear")]
        CLS,
        [CodeDescriptorAttribue("COMM", "Communicate", "*")]
        COMM,
        [CodeDescriptorAttribue("COMP", "Compensate")]
        COMP,
        [CodeDescriptorAttribue("COND", "Condition")]
        COND,
        [CodeDescriptorAttribue("CONF", "Configuration")]
        CONF,
        [CodeDescriptorAttribue("CONT", "Controller")]
        CONT,
        [CodeDescriptorAttribue("CONV", "Convert")]
        CONV,
        [CodeDescriptorAttribue("CORR", "Correction")]
        CORR,
        [CodeDescriptorAttribue("COUN", "Count")]
        COUN,
        [CodeDescriptorAttribue("DEF", "Define")]
        DEF,
        [CodeDescriptorAttribue("DIAG", "Diagnostic")]
        DIAG,
        [CodeDescriptorAttribue("DIOD", "Diode")]
        DIOD,
        [CodeDescriptorAttribue("DISP", "Display")]
        DISP,
        [CodeDescriptorAttribue("EAV", "Error in error queue")]
        EAV,
        [CodeDescriptorAttribue("EFF", "Effort")]
        EFF,
        [CodeDescriptorAttribue("ENAB", "Enable")]
        ENAB,
        [CodeDescriptorAttribue("EOI", "End of input")]
        EOI,
        [CodeDescriptorAttribue("ERR", "Error")]
        ERR,
        [CodeDescriptorAttribue("ESB", "Summary bit from standard event")]
        ESB,
        [CodeDescriptorAttribue("ESE", "Event status enable")]
        ESE,
        [CodeDescriptorAttribue("ESR", "Event status register", "*")]
        ESR,
        [CodeDescriptorAttribue("EVEN", "Event", "*")]
        EVEN,
        [CodeDescriptorAttribue("FILT", "Filter")]
        FILT,
        [CodeDescriptorAttribue("FREQ", "Frequency")]
        FREQ,
        [CodeDescriptorAttribue("FULL", "Fullscale")]
        FULL,
        [CodeDescriptorAttribue("GTL", "Go to local")]
        GTL,
        [CodeDescriptorAttribue("HEAD", "Head")]
        HEAD,
        [CodeDescriptorAttribue("IDN", "Identification", "*")]
        IDN,
        [CodeDescriptorAttribue("IMM", "Immediate")]
        IMM,
        [CodeDescriptorAttribue("INL", "In limit")]
        INL,
        [CodeDescriptorAttribue("INP", "Input")]
        INP,
        [CodeDescriptorAttribue("INST", "Instrument")]
        INST,
        [CodeDescriptorAttribue("ISOL", "Isolation")]
        ISOL,
        [CodeDescriptorAttribue("LEV", "Leve")]
        LEV,
        [CodeDescriptorAttribue("LIM", "Limit")]
        LIM,
        [CodeDescriptorAttribue("LLO", "Local lock out")]
        LLO,
        [CodeDescriptorAttribue("LOG", "Logical")]
        LOG,
        [CodeDescriptorAttribue("LPAS", "Low pass (filter)")]
        LPAS,
        [CodeDescriptorAttribue("MAV", "Message available in output queue")]
        MAV,
        [CodeDescriptorAttribue("MEAS", "Measure")]
        MEAS,
        [CodeDescriptorAttribue("MSS", "Summary bit after SRQ")]
        MSS,
        [CodeDescriptorAttribue("NEGC", "Negative Calibration")]
        NEGC,
        [CodeDescriptorAttribue("OFFS", "Offset")]
        OFFS,
        [CodeDescriptorAttribue("OPC", "Operational condition", "*")]
        OPC,
        [CodeDescriptorAttribue("OPER", "Operation", "*")]
        OPER,
        [CodeDescriptorAttribue("OPT", "Option")]
        OPT,
        [CodeDescriptorAttribue("OSB", "Summary bit from standard operations status register")]
        OSB,
        [CodeDescriptorAttribue("OUTP", "Output")]
        OUTP,
        [CodeDescriptorAttribue("OVER", "Overshoot")]
        OVER,
        [CodeDescriptorAttribue("PAR", "Parity")]
        PAR,
        [CodeDescriptorAttribue("PASS", "Password")]
        PASS,
        [CodeDescriptorAttribue("PERC", "Percent")]
        PERC,
        [CodeDescriptorAttribue("POIN", "Points")]
        POIN,
        [CodeDescriptorAttribue("PRES", "Preset")]
        PRESet,
        [CodeDescriptorAttribue("PRES", "Pressure")]
        PRESsure,
        [CodeDescriptorAttribue("QUE", "Queue")]
        QUE,
        [CodeDescriptorAttribue("QUES", "Questionable")]
        QUES,
        [CodeDescriptorAttribue("RANG", "Range")]
        RANG,
        [CodeDescriptorAttribue("REF", "Reference")]
        REF,
        [CodeDescriptorAttribue("RES", "Resolution")]
        RESolution,
        [CodeDescriptorAttribue("RES", "RESet")]
        RESet,
        [CodeDescriptorAttribue("SAMP", "Sample")]
        SAMP,
        [CodeDescriptorAttribue("SENS", "Sense")]
        SENS,
        [CodeDescriptorAttribue("SEPT", "Set-point")]
        SEPT,
        [CodeDescriptorAttribue("SER", "Serial")]
        SER,
        [CodeDescriptorAttribue("SOUR", "Source")]
        SOUR,
        [CodeDescriptorAttribue("SPE", "Speed")]
        SPE,
        [CodeDescriptorAttribue("SRE", "Service request enable", "*")]
        SRE,
        [CodeDescriptorAttribue("SRQ", "Service request")]
        SRQ,
        [CodeDescriptorAttribue("STAR", "Start")]
        STAR,
        [CodeDescriptorAttribue("STAT", "State")]
        STAT,
        [CodeDescriptorAttribue("STB", "Status register query", "*")]
        STB,
        [CodeDescriptorAttribue("SYST", "System")]
        SYST,
        [CodeDescriptorAttribue("TIM", "Time to set-point")]
        TIM,
        [CodeDescriptorAttribue("TST", "Queries the self-test status", "*")]
        TST,
        [CodeDescriptorAttribue("UNIT", "Unit of pressure")]
        UNIT,
        [CodeDescriptorAttribue("VAL", "Value")]
        VAL,
        [CodeDescriptorAttribue("VALV", "Valve")]
        VALV,
        [CodeDescriptorAttribue("VERS", "Version")]
        VERS,
        [CodeDescriptorAttribue("VOL", "Volume")]
        VOL,
        [CodeDescriptorAttribue("WAI", "Stops further commands from being carried out until the completion of all pending operations", "*")]
        WAI,
    }
}
