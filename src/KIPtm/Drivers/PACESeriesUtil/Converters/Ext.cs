using System;
using System.Collections.Generic;
using System.Globalization;
using PACESeries;

namespace PACESeriesUtil.Converters
{
    public static class Ext
    {
        public static string ToLocalizedString(this PressureUnits unti)
        {
            var dict = new Dictionary<PressureUnits, string>()
            {
                {PressureUnits.None, Properties.Resources.PressureUnits_None},
                {PressureUnits.MBar, Properties.Resources.PressureUnits_MBar},
                {PressureUnits.Bar, Properties.Resources.PressureUnits_Bar},
                {PressureUnits.inH2O4, Properties.Resources.PressureUnits_inH2O4},
                {PressureUnits.inH2O, Properties.Resources.PressureUnits_inH2O},
                {PressureUnits.inHg, Properties.Resources.PressureUnits_inHg},
                {PressureUnits.mmHg, Properties.Resources.PressureUnits_mmHg},
                {PressureUnits.Pa, Properties.Resources.PressureUnits_Pa},
                {PressureUnits.hPa, Properties.Resources.PressureUnits_hPa},
                {PressureUnits.kPa, Properties.Resources.PressureUnits_kPa},
                {PressureUnits.psi, Properties.Resources.PressureUnits_psi},
                {PressureUnits.inH2O60F, Properties.Resources.PressureUnits_inH2O60F},
                {PressureUnits.KgCm2, Properties.Resources.PressureUnits_KgCm2},
                {PressureUnits.ATM, Properties.Resources.PressureUnits_ATM},
                {PressureUnits.mmH2O4, Properties.Resources.PressureUnits_mmH2O4},
            };

            if(!dict.ContainsKey(unti))
                throw new ArgumentOutOfRangeException(nameof(unti), unti, null);
            return dict[unti];
        }

        //public 
    }
}
