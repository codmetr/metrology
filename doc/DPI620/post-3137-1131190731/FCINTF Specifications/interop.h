/***************************************************************************
 * SourceSafe Header
 *
 * $Workfile: interop.h $
 * $Revision: 6 $
 * $Date: 7/07/99 10:42a $
 *
 ***************************************************************************/

/***************************************************************************
 * Copyright 1998-1999, Southwest Research Institute.
 ***************************************************************************/

/***************************************************************************
 * File Description:
 *  This file contains definitions from the HART common tables that are
 * designed to enhance the interoperability of HART devices.
 *
 ***************************************************************************/

/***************************************************************************
 * $History: interop.h $
 * 
 * *****************  Version 6  *****************
 * User: Kholladay    Date: 7/07/99    Time: 10:42a
 * Updated in $/FieldCalibrators/fcintf/include
 * Latest unit additions
 *
 * *****************  Version 5  *****************
 * User: Kholladay    Date: 3/25/99    Time: 11:12a
 * Updated in $/FieldCalibrators/fcintf/include
 * Added 3 new units.
 *
 * *****************  Version 4  *****************
 * User: Kholladay    Date: 12/01/98   Time: 1:07p
 * Updated in $/FieldCalibrators/fcintf/include
 *
 * *****************  Version 3  *****************
 * User: Kholladay    Date: 11/23/98   Time: 4:10p
 * Updated in $/FieldCalibrators/fcintf/include
 *
 *
 ***************************************************************************/


#ifndef INTEROP_H
#define INTEROP_H

/***************************************************************************
 * HART Table 2 - Unit Codes
 ***************************************************************************/

#define UNIT_INH2O              1   // inches of water @ 68degF
#define UNIT_INHG               2   // inches of mercury @ 0 degC
#define UNIT_FTH2O              3   // feet of water @ 68 degF
#define UNIT_MMH2O              4   // millimeters of water @ 68 degF
#define UNIT_MMHG               5   // millimeters of mercury @ 0 degC
#define UNIT_PSI                6   // pounds per square inch
#define UNIT_BAR                7   // bars
#define UNIT_MBAR               8   // millibars
#define UNIT_G_PER_SQCM         9   // grams per square centimeter
#define UNIT_KG_PER_SQCM       10   // kilograms per square centimeter
#define UNIT_PA                11   // pascals
#define UNIT_KPA               12   // kilopascals
#define UNIT_TORR              13   // torr
#define UNIT_ATM               14   // atmospheres
#define UNIT_CUFT_PER_MIN      15   // cubic feet per minute
#define UNIT_GAL_PER_MIN       16   // gallons per minute
#define UNIT_LITERS_PER_MIN    17   // liters per minute
#define UNIT_IMPGAL_PER_MIN    18   // imperial gallons per minute
#define UNIT_CUMTR_PER_HR      19   // cubic meters per hour
#define UNIT_FT_PER_S          20   // feet per second
#define UNIT_METER_PER_S       21   // meters per second
#define UNIT_GAL_PER_S         22   // gallons per second
#define UNIT_MILGAL_PER_DAY    23   // million gallons per day
#define UNIT_LITERS_PER_S      24   // liters per second
#define UNIT_MIL_L_PER_DAY     25   // million liters per day
#define UNIT_CUFT_PER_S        26   // cubic feet per second
#define UNIT_CUFT_PER_DAY      27   // cubic feet per day
#define UNIT_CUMTR_PER_S       28   // cubic meters per second
#define UNIT_CUMTR_PER_DAY     29   // cubic meters per day
#define UNIT_IMPGAL_PER_HR     30   // imperial gallons per hour
#define UNIT_IMPGAL_PER_DAY    31   // imperial gallons per day
#define UNIT_DEGC              32   // degrees Cesius
#define UNIT_DEGF              33   // degrees Fahrenheit
#define UNIT_DEGR              34   // degrees Rankine
#define UNIT_KELVIN            35   // degrees Kelvin
#define UNIT_MV                36   // millivolts
#define UNIT_OHM               37   // ohms
#define UNIT_HZ                38   // hertz
#define UNIT_MA                39   // milliamps
#define UNIT_GAL               40   // gallons
#define UNIT_LITER             41   // liters
#define UNIT_IMPGAL            42   // imperial gallons
#define UNIT_CUMTR             43   // cubic meters
#define UNIT_FT                44   // feet
#define UNIT_METER             45   // meters
#define UNIT_BBL               46   // barrels (42 gallons)
#define UNIT_IN                47   // inches
#define UNIT_CM                48   // centimeters
#define UNIT_MM                49   // millimeters
#define UNIT_MIN               50   // minutes
#define UNIT_SEC               51   // seconds
#define UNIT_HR                52   // hours
#define UNIT_DAY               53   // days
#define UNIT_CSTOKE            54   // centistokes
#define UNIT_CPOISE            55   // centipoise
#define UNIT_UMHO              56   // microsiemens
#define UNIT_PCT               57   // percent
#define UNIT_V                 58   // volts
#define UNIT_PH                59   // pH
#define UNIT_GRAM              60   // grams
#define UNIT_KG                61   // kilograms
#define UNIT_METTON            62   // metric tons
#define UNIT_LB                63   // pounds
#define UNIT_SHTON             64   // short tons
#define UNIT_LTON              65   // long tons
#define UNIT_MSIEMEN_PER_CM    66   // millisiemens per centimeter
#define UNIT_USIEMEN_PER_CM    67   // microsiemens per centimeter
#define UNIT_NEWTON            68   // Newton
#define UNIT_NEWTMETER         69   // newton meter
#define UNIT_G_PER_S           70   // grams per second
#define UNIT_G_PER_MIN         71   // grams per minute
#define UNIT_G_PER_HR          72   // grams per hour
#define UNIT_KG_PER_S          73   // kilograms per second
#define UNIT_KG_PER_MIN        74   // kilograms per minute
#define UNIT_KG_PER_HR         75   // kilograms per hour
#define UNIT_KG_PER_DAY        76   // kilograms per day
#define UNIT_METTON_PER_MIN    77   // metric tons per minute
#define UNIT_METTON_PER_HR     78   // metric tons per hour
#define UNIT_METTON_PER_DAY    79   // metric tons per day
#define UNIT_LB_PER_S          80   // pounds per second
#define UNIT_LB_PER_MIN        81   // pounds per minute
#define UNIT_LB_PER_HR         82   // pounds per hour
#define UNIT_LB_PER_DAY        83   // pounds per day
#define UNIT_SHTON_PER_MIN     84   // short tons per minute
#define UNIT_SHTON_PER_HR      85   // short tons per hour
#define UNIT_SHTON_PER_DAY     86   // short tons per day
#define UNIT_LTON_PER_HR       87   // long tons per hour
#define UNIT_LTON_PER_DAY      88   // long tons per day
#define UNIT_DATHERM           89   // deka therm
#define UNIT_SGU               90   // specific gravity unit
#define UNIT_G_PER_CUCM        91   // grams per cubic centimeter
#define UNIT_KG_PER_CUMTR      92   // kilograms per cubic centimeter
#define UNIT_LB_PER_GAL        93   // pounds per gallon
#define UNIT_LB_PER_CUFT       94   // pounds per cubic foot
#define UNIT_G_PER_ML          95   // grams per milliliter
#define UNIT_KG_PER_L          96   // kilograms per liter
#define UNIT_G_PER_L           97   // grams per liter
#define UNIT_LB_PER_CUIN       98   // pounds per cubic inch
#define UNIT_SHTON_PER_CUYD    99   // short tons per cubic yard
#define UNIT_DEGTWAD          100   // degrees twaddell
#define UNIT_DEGBRIX          101   // degree brix
#define UNIT_DEGBAUM_HV       102   // degrees baume heavy
#define UNIT_DEGBAUM_LT       103   // degrees baume light
#define UNIT_DEGAPI           104   // degrees API (Am Petroleum Inst)
#define UNIT_PCTSOL_WT        105   // percent solids by weight
#define UNIT_PCTSOL_VOL       106   // percent solids by volume
#define UNIT_DEGBALL          107   // degrees balling
#define UNIT_PROOF_PER_VOL    108   // proof per volume
#define UNIT_PROOF_PER_MASS   109   // proof per mass
#define UNIT_BUSH             110   // bushels
#define UNIT_CUYD             111   // cubic yards
#define UNIT_CUFT             112   // cubic feet
#define UNIT_CUIN             113   // cubic inches
#define UNIT_IN_PER_SEC       114   // inches per second
#define UNIT_IN_PER_MIN       115   // inches per minute
#define UNIT_FT_PER_MIN       116   // feet per minute
#define UNIT_DEG_PER_SEC      117   // degrees per second
#define UNIT_REV_PER_SEC      118   // revolutions per second
#define UNIT_RPM              119   // revolutions per minute
#define UNIT_MTR_PER_HR       120   // meters per hour
#define UNIT_NMLCUMTR_PER_HR  121   // normal cubic meter per hour
#define UNIT_NMLLITER_PER_HR  122   // normal liter per hour
#define UNIT_STDCUFT_PER_MIN  123   // standard cubic feet per minute
#define UNIT_BARRELS_LIQ      124   // liquid barrels (31.5 gal)
#define UNIT_OUNCE            125   // ounces
#define UNIT_FT_LBF           126   // foot pound force
#define UNIT_KW               127   // kilowatt
#define UNIT_KWH              128   // kilowatt hour
#define UNIT_HP               129   // horsepower
#define UNIT_CUFT_PER_HR      130   // cubic feet per hour
#define UNIT_CUMTR_PER_MIN    131   // cubic meters per minute
#define UNIT_BBL_PER_SEC      132   // barrels per second
#define UNIT_BBL_PER_MIN      133   // barrels per minute
#define UNIT_BBL_PER_HR       134   // barrels per hour
#define UNIT_BBL_PER_DAY      135   // barrels per day
#define UNIT_GAL_PER_HR       136   // gallons per hour
#define UNIT_IMPGAL_PER_SEC   137   // imperial gallons per second
#define UNIT_L_PER_HR         138   // liters per hour
#define UNIT_PPM              139   // parts per million
#define UNIT_MCAL_PER_HR      140   // mega calories per hour
#define UNIT_MJOULE_PER_HR    141   // mega joules per hour
#define UNIT_BTU_PER_HR       142   // british thermal units per hour
#define UNIT_DEG              143   // degrees
#define UNIT_RAD              144   // radians
#define UNIT_INH2O_60DEGF     145   // inches of water @ 60 degF
#define UNIT_UG_PER_L         146   // micrograms per liter
#define UNIT_UG_PER_M3        147   // micrgrams per cubic meter
#define UNIT_PCT_CONSIST      148   // Percent Consistency for Pulp and Paper
#define UNIT_VOL_PCT          149   // Volume percent
#define UNIT_PCT_STM_QUAL     150   // percent steam quality
#define UNIT_FT_IN_16THS      151   // Feet in sixteenths
#define UNIT_CUFT_PER_LB      152   // cubic feet per pound
#define UNIT_PF               153   // picofarads
#define UNIT_ML_PER_L         154   // milliliters per liter
#define UNIT_UL_PER_L         155   // microliters per liter
#define UNIT_PCT_PLATO        160   // percent plato
#define UNIT_PCT_LEL          161   // % lower explosive limit
#define UNIT_MCAL             162   // mega calories
#define UNIT_KOHM             163   // kiloOhm
#define UNIT_MJOULES          164   // mega joule
#define UNIT_BTU              165   // British thermal unit
#define UNIT_NMLCUMTR         166   // normal cubic meter
#define UNIT_NMLLITER         167   // normal liter
#define UNIT_STDCUFT          168   // standard cubic feet
#define UNIT_PPB              169   // parts per billion
#define UNIT_GAL_PER_DAY      235   // gallons per day
#define UNIT_HLITER           236   // hectoliter
#define UNIT_MPA              237   // megapascals
#define UNIT_INH2O_4DEGC      238   // inches of water @ 4degC
#define UNIT_MMH2O_4DEGC      239   // millimeters water @ 4degC
#define UNIT_NOT_USED         250
#define UNIT_NONE             251
#define UNIT_UNKNOWN          252
#define UNIT_SPECIAL          253




/***************************************************************************
 * HART Table 3 - Transfer Function (Relationship) Codes
 ***************************************************************************/

/* Equation comments are x as %input vs.  y as %output */
#define RELATION_LINEAR                   0  // y = x
#define RELATION_SQRT                     1  // y = sqrt( x ) * 10.0
#define RELATION_SQRT_3RD_PWR             2  // y = sqrt(x^3) * 0.1
#define RELATION_SQRT_5TH_PWR             3  // y = sqrt(x^5) * 0.001
#define RELATION_TABLE                    4  // special curve
#define RELATION_SQUARE                   5  // y = x^2 * 0.01
#define RELATION_SWITCH                 230  // discreet output
#define RELATION_SQRT_SPECIAL           231  // + table
#define RELATION_SQRT_THIRD_SPECIAL     232  // + table
#define RELATION_SQRT_FIFTH_SPECIAL     233  // + table
#define RELATION_NOT_USED               250
#define RELATION_NONE                   251
#define RELATION_UNKNOWN                252

/***************************************************************************
 * HART Table 16 - Temperature Probe Definitions
 ***************************************************************************/

#define PROBE_OHMS                   1  // Ohms
#define PROBE_KOHMS                  2  // kiloOhms
#define PROBE_RTD_CALIBRATED         3  // must supply Calendar-VanDeusen parameters.

#define PROBE_RTD_PT50_385          11  // RTD Pt   50    a=0.003850  (IEC751)
#define PROBE_RTD_PT100_385         12  // RTD Pt  100    a=0.003850
#define PROBE_RTD_PT200_385         13  // RTD Pt  200    a=0.003850
#define PROBE_RTD_PT500_385         14  // RTD Pt  500    a=0.003850
#define PROBE_RTD_PT1000_385        15  // RTD Pt 1000    a=0.003850

#define PROBE_RTD_PT50_3916         21  // RTD Pt  50     a=0.003916  (JIS C1604-81)
#define PROBE_RTD_PT100_3916        22  // RTD Pt  100    a=0.003916

#define PROBE_RTD_PT50_392          31  // RTD Pt   50    a=0.003920  (MIL-T-24388)
#define PROBE_RTD_PT100_392         32  // RTD Pt  100    a=0.003920
#define PROBE_RTD_PT200_392         33  // RTD Pt  200    a=0.003920
#define PROBE_RTD_PT500_392         34  // RTD Pt  500    a=0.003920
#define PROBE_RTD_PT1000_392        35  // RTD Pt 1000    a=0.003920

#define PROBE_RTD_PT10_3923         40  // RTD Pt   10    a=0.003923  (SAMA RC21-4-1966)
#define PROBE_RTD_PT100_3923        41  // RTD Pt  100    a=0.003923
#define PROBE_RTD_PT200_3923        42  // RTD Pt  200    a=0.003923

#define PROBE_RTD_PT100_3926        51  // RTD Pt  100    a=0.003926  (IPTS-68)

#define PROBE_RTD_NI50_672          61  // RTD Ni   50    a=0.006720  (Edison curve #7)
#define PROBE_RTD_NI100_672         62  // RTD Ni  100    a=0.006720
#define PROBE_RTD_NI120_672         63  // RTD Ni  120    a=0.006720
#define PROBE_RTD_NI1000_672        64  // RTD Ni 1000    a=0.006720

#define PROBE_RTD_NI50_618          71  // RTD Ni   50    a=0.006180  (DIN 43760)
#define PROBE_RTD_NI100_618         72  // RTD Ni  100    a=0.006180
#define PROBE_RTD_NI120_618         73  // RTD Ni  120    a=0.006180
#define PROBE_RTD_NI1000_618        74  // RTD Ni 1000    a=0.006180

#define PROBE_RTD_CU10_427          80  // RTD Cu   10    a=0.004270  (?? Standard ??)
#define PROBE_RTD_CU100_427         81  // RTD Cu  100    a=0.004270

#define PROBE_MICROVOLT            128  // microVolts
#define PROBE_MILLIVOLT            129  // milliVolts
#define PROBE_VOLT                 130  // Volts

                                        // Thermocouples.
                                        // Unless otherwise noted, thermocouple references are:
                                        // IEC 584, NIST MN 175, DIN 43710, BS 4937,
                                        // ANSI MC96.1, JIS C1602 and NF C42-321)
#define PROBE_TC_B                 131  // TC Type B (Pt30Rh-Pt6Rh) (IEC 584 etc.)
#define PROBE_TC_C_W5              132  // TC Type W5, Omega type C (W5-W26Rh)(ASTM E 988)
#define PROBE_TC_D_W3              133  // TC Type W3, Omega type D (W3-W25Rh)(ASTM E 988 )
#define PROBE_TC_E                 134  // TC Type E (Ni10Cr-Cu45Ni) (IEC 584 etc.)
#define PROBE_TC_G_W               135  // TC Type W, Omega type G (W-W26Rh)(ASTM E 988)
#define PROBE_TC_J                 136  // TC Type J (Fe-Cu45Ni) (IEC 584 etc.)
#define PROBE_TC_K                 137  // TC Type K (Ni10Cr-Ni5) (IEC 584 etc.)
#define PROBE_TC_N                 138  // TC Type N (Ni14CrSi-NiSi) (IEC 584 etc.)
#define PROBE_TC_R                 139  // TC Type R (Pt13Rh-Pt) (IEC 584 etc.)
#define PROBE_TC_S                 140  // TC Type S (Pt10Rh-Pt) (IEC 584 etc.)
#define PROBE_TC_T                 141  // TC Type T (Cu-Cu45Ni) (IEC 584 etc.)
#define PROBE_TC_L                 142  // TC Type L (Fe-CuNi) (DIN 43710)
#define PROBE_TC_U                 143  // TC Type U (Cu-CuNi) (DIN 43710)
#define PROBE_TC_PT20RH            144  // TC Pt20Rh-Pt40Rh (ASTM E 1751)
#define PROBE_TC_IR40              145  // TC Ir-Ir40Rh (ASTM E 1751)
#define PROBE_TC_PLATINEL          146  // TC Platinel II
#define PROBE_TC_NI_NIMO           147  // TC Ni-NiMo

#define PROBE_BIMETALLIC           220  // Bi-metallic strip
#define PROBE_VP_BULB              221  // Vapor pressure bulb
#define PROBE_LIQUID_EXP           222  // Liquid expansion
#define PROBE_MERCURY_BULB         223  // Mercury bulb

#define PROBE_NOT_USED             250  // Not used
#define PROBE_NONE                 251  // None
#define PROBE_UNKNOWN              252  // Unknown
#define PROBE_SPECIAL              253  // Special


/***************************************************************************
 * HART Table 17 - Temperature Probe Connections
 ***************************************************************************/

#define CNCT_SINGLE                  1  // Single.  One probe is used to measure temperature.
#define CNCT_DIFFERENTIAL            2  // Differential.  Two probes are connected in series
                                        //   to read differential temperature.

#define CNCT_NOT_USED              250  // Not used
#define CNCT_NONE                  251  // None
#define CNCT_UNKNOWN               252  // Unknown
#define CNCT_SPECIAL               253  // Special

/***************************************************************************
 * HART Table 18 - Thermocouple Cold Junction Compensation (CJC) Options
 ***************************************************************************/

#define TC_CJC_INTERNAL              1  // A sensor built into the device provides CJC
#define TC_CJC_EXTERNAL              2  // An external sensor must be used
#define TC_CJC_MANUAL                3  // A fixed value is supplied

#define TC_CJC_NOT_USED            250  // Not used
#define TC_CJC_NONE                251  // None
#define TC_CJC_UNKNOWN             252  // Unknown
#define TC_CJC_SPECIAL             253  // Special

/***************************************************************************
 * HART Table 19 - Pressure Measurement Types
 ***************************************************************************/

#define PRESSURE_TYPE_DIFFERENTIAL      0  // Differential pressure measurement
#define PRESSURE_TYPE_GAUGE             1  // Gauge pressure measurement
#define PRESSURE_TYPE_ABSOLUTE          2  // Absolute pressure measurement
#define PRESSURE_TYPE_HIGH_LINE         3  // High line pressure measurement
#define PRESSURE_TYPE_LIQUID_LEVEL      4  // Liquid level pressure measurement
#define PRESSURE_TYPE_DRAFT_RANGE       5  // Draft range pressure measurement
#define PRESSURE_TYPE_HTG               6  // Hydrostatic Tank Gauge

#define PRESSURE_TYPE_NOT_USED        250  // Not used
#define PRESSURE_TYPE_NONE            251  // None
#define PRESSURE_TYPE_UNKNOWN         252  // Unknown
#define PRESSURE_TYPE_SPECIAL         253  // Special

/***************************************************************************
 * HART Table 20 - Frequency Wave Forms
 ***************************************************************************/

#define WAVEFORM_SQUARE              0  // Square wave
#define WAVEFORM_SINE                1  // Sine wave
#define WAVEFORM_TRIANGLE            2  // Triangle wave
#define WAVEFORM_SAWTOOTH            3  // Sawtooth wave

#define WAVEFORM_NOT_USED          250  // Not used
#define WAVEFORM_NONE              251  // None
#define WAVEFORM_UNKNOWN           252  // Unknown
#define WAVEFORM_SPECIAL           253  // Special

#endif /* INTEROP_H */
