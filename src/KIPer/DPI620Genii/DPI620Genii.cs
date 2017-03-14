using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPI620Genii
{
    public class DPI620Genii
    {
        private readonly IEnumerable<ParamId> Ids = new List<ParamId>()
        {
            new ParamId(null, ParamId.ParamType.Humidity, 0, 0),
            new ParamId(TUnit.C, ParamId.ParamType.Temperature, 32, 0),
            new ParamId(TUnit.C, ParamId.ParamType.TemperatureRTD, 32, 0),
            new ParamId(TUnit.C, ParamId.ParamType.TemperatureTC, 32, 0),
            new ParamId(TUnit.F, ParamId.ParamType.Temperature, 33, 0),
            new ParamId(TUnit.F, ParamId.ParamType.TemperatureRTD, 33, 0),
            new ParamId(TUnit.F, ParamId.ParamType.TemperatureTC, 33, 0),
            new ParamId(TUnit.R, ParamId.ParamType.Temperature, 34, 0),
            new ParamId(TUnit.R, ParamId.ParamType.TemperatureRTD, 34, 0),
            new ParamId(TUnit.R, ParamId.ParamType.TemperatureTC, 34, 0),
            new ParamId(PUnit.atm, ParamId.ParamType.Pressure, 14, 15),
            new ParamId(PUnit.bar, ParamId.ParamType.Pressure, 7, 4),
            new ParamId(PUnit.cmH2O_4C, ParamId.ParamType.Pressure, 183, 0),
            new ParamId(PUnit.cmHg_0C, ParamId.ParamType.Pressure, 172, 0),
            new ParamId(PUnit.cmH2O, ParamId.ParamType.Pressure, 176, 0),
            new ParamId(PUnit.cmHg, ParamId.ParamType.Pressure, 170, 0),
            new ParamId(FUnit.CPH, ParamId.ParamType.Frequency, 192, 0),
            new ParamId(FUnit.CPM, ParamId.ParamType.Frequency, 191, 0),
            new ParamId(PUnit.ftH20_20C, ParamId.ParamType.Pressure, 186, 0),
            new ParamId(PUnit.ftH2O, ParamId.ParamType.Pressure, 3, 0),
            new ParamId(PUnit.ftH2O_4C, ParamId.ParamType.Pressure, 181, 0),
            new ParamId(PUnit.ftH2O_60C, ParamId.ParamType.Pressure, 182, 0),
            new ParamId(null, ParamId.ParamType.Density, 0, 0),
            new ParamId(FUnit.hertz, ParamId.ParamType.Frequency, 38, 0),
            new ParamId(PUnit.hPa, ParamId.ParamType.Pressure, 179, 0),
            new ParamId(PUnit.inH2O_0C, ParamId.ParamType.Pressure, 185, 0),
            new ParamId(PUnit.inH2O_20C, ParamId.ParamType.Pressure, 1, 0),
            new ParamId(PUnit.inH2O_25C, ParamId.ParamType.Pressure, 184, 0),
            new ParamId(PUnit.inH2O_4C, ParamId.ParamType.Pressure, 238, 0),
            new ParamId(PUnit.inH2O_60C, ParamId.ParamType.Pressure, 180, 0),
            new ParamId(PUnit.inH2O, ParamId.ParamType.Pressure, 173, 0),
            new ParamId(PUnit.inH2O_60F, ParamId.ParamType.Pressure, 145, 0),
            new ParamId(PUnit.inHg, ParamId.ParamType.Pressure, 2, 0),
            new ParamId(PUnit.inHg_0C, ParamId.ParamType.Pressure, 2, 0),
            new ParamId(PUnit.inHg_60F, ParamId.ParamType.Pressure, 171, 0),
            new ParamId(TUnit.K, ParamId.ParamType.Temperature, 35, 0),
            new ParamId(TUnit.K, ParamId.ParamType.TemperatureRTD, 35, 0),
            new ParamId(TUnit.K, ParamId.ParamType.TemperatureTC, 35, 0),
            new ParamId(PUnit.kgcm2, ParamId.ParamType.Pressure, 10, 0),
            new ParamId(PUnit.kgm2, ParamId.ParamType.Pressure, 178, 0),
            new ParamId(PUnit.kgfcm2, ParamId.ParamType.Pressure, 10, 0),
            new ParamId(FUnit.kHertz, ParamId.ParamType.Frequency, 190, 0),
            new ParamId(RUnit.kOhm, ParamId.ParamType.Resistance, 163, 0),
            new ParamId(PUnit.kPa, ParamId.ParamType.Pressure, 12, 8),
            new ParamId(PUnit.lbft2, ParamId.ParamType.Pressure, 177, 0),
            new ParamId(null, ParamId.ParamType.Current, 39, 0),
            new ParamId(PUnit.mbar, ParamId.ParamType.Pressure, 8, 3),
            new ParamId(PUnit.mH2O, ParamId.ParamType.Pressure, 175, 0),
            new ParamId(PUnit.mHg, ParamId.ParamType.Pressure, 174, 0),
            new ParamId(PUnit.mmH2O_4C, ParamId.ParamType.Pressure, 239, 0),
            new ParamId(PUnit.mmH2O, ParamId.ParamType.Pressure, 4, 0),
            new ParamId(PUnit.mmHg, ParamId.ParamType.Pressure, 5, 0),
            new ParamId(PUnit.mmHg_0C, ParamId.ParamType.Pressure, 5, 0),
            new ParamId(PUnit.MPa, ParamId.ParamType.Pressure, 237, 9),
            new ParamId(VUnit.mV, ParamId.ParamType.Voltage, 36, 0),
            new ParamId(VUnit.mV_AC, ParamId.ParamType.Voltage, 194, 0),
            new ParamId(RUnit.ohm, ParamId.ParamType.Resistance, 37, 0),
            new ParamId(PUnit.Pa, ParamId.ParamType.Pressure, 11, 7),
            new ParamId(PUnit.psi, ParamId.ParamType.Pressure, 6, 14),
            new ParamId(PUnit.Torr, ParamId.ParamType.Pressure, 13, 16),
            new ParamId(null, ParamId.ParamType.Observed, 252, 0),
            new ParamId(VUnit.V, ParamId.ParamType.Voltage, 58, 0),
            new ParamId(VUnit.V_AC, ParamId.ParamType.Voltage, 193, 0),
        };

        /*
        UnitsName	    Parameter	UnitNumber	UnitID
         %RH	        Humidity	        0	0
        °C  	        Temperature	        32	0
        °C  	        Temperature (RTD)	32	0
        °C  	        Temperature (TC)	32	0
        °F  	        Temperature	        33	0
        °F  	        Temperature (RTD)	33	0
        °F  	        Temperature (TC)	33	0
        °R  	        Temperature	        34	0
        °R  	        Temperature (RTD)	34	0
        °R  	        Temperature (TC)	34	0
        atm 	        Pressure	        14	15
        bar 	        Pressure	        7	4
        cm H2O @ 4°C	Pressure	        183	0
        cm Hg @ 0°C 	Pressure	        172	0
        cmH2O	        Pressure	        176	0
        cmHg	        Pressure	        170	0
        CPH 	        Frequency	        192	0
        CPM 	        Frequency	        191	0
        ftH20 @ 20 °C	Pressure	        186	0
        ftH2O	        Pressure	        3	0
        ftH2O @ 4°C	    Pressure	        181	0
        ftH2O @ 60°C	Pressure	        182	0
        g/cm³	        Density 	        0	0
        hertz	        Frequency	        38	0
        hPa	            Pressure	        179	0
        in H2O @ 0°C	Pressure	        185	0
        in H2O @ 20°C	Pressure	        1	0
        in H2O @ 25°C	Pressure	        184	0
        in H2O @ 4°C	Pressure	        238	0
        in H2O @ 60°C	Pressure	        180	0
        inH2O	        Pressure	        173	0
        inH2O @ 60°F	Pressure	        145	0
        inHg	        Pressure	        2	0
        inHg @ 0°C	    Pressure	        2	0
        inHg @ 60°F	    Pressure	        171	0
        K	            Temperature	        35	0
        K	            Temperature (RTD)	35	0
        K	            Temperature (TC)	35	0
        kg/cm2	        Pressure	        10	0
        kg/m2	        Pressure	        178	0
        kgf/cm2	        Pressure	        10	0
        kHertz	        Frequency	        190	0
        kOhm	        Resistance	        163	0
        kPa	            Pressure	        12	8
        lb/ft2	        Pressure	        177	0
        mA	            Current 	        39	0
        mbar	        Pressure	        8	3
        mH2O	        Pressure	        175	0
        mHg	            Pressure	        174	0
        mm H2O @ 4°C	Pressure	        239	0
        mmH2O	        Pressure	        4	0
        mmHg	        Pressure	        5	0
        mmHg @ 0°C	    Pressure	        5	0
        MPa	            Pressure	        237	9
        mV	            Voltage 	        36	0
        mV (AC)	        Voltage 	        194	0
        ohm	            Resistance	        37	0
        Pa	            Pressure	        11	7
        psi	            Pressure	        6	14
        Torr	        Pressure	        13	16
        User Unit	    Observed	        252	0
        V	            Voltage 	        58	0
        V (AC)	        Voltage 	        193	0
         */
        
        public double Humidity { get; set; }

        public double Temperature { get; set; }

        public TUnit TemperatureUnit { get; set; }

        public double TemperatureRTD { get; set; }

        public TUnit TemperatureRTDUnit { get; set; }

        public double TemperatureTC { get; set; }

        public TUnit TemperatureTCUnit { get; set; }

        public double Pressure { get; set; }

        public PUnit PressureUnit { get; set; }

        public double Frequency { get; set; }

        public FUnit FrequencyUnit { get; set; }

        public double Density { get; set; }

        public double Resistance { get; set; }

        public RUnit ResistanceUnit { get; set; }

        public double Current { get; set; }

        public double Voltage { get; set; }

        public VUnit VoltageUnit { get; set; }

        public UserUnit Observed { get; set; }
    }
}
