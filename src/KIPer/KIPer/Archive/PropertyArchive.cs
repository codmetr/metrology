using System.Collections.Generic;
using ADTS;
using KipTM.Archive.DataTypes;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Archive
{
    public class PropertyArchive : ArchiveBase
    {
        public static ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        public PropertyArchive()
            : base()
        { }

        public PropertyArchive(List<ArchivedKeyValuePair> data)
            : base(data)
        { }

        #region GetDefault
        public static List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSModel.Key, GetDefaultForADTS()),
            };
        }

        public static List<ArchivedKeyValuePair> GetDefaultForADTS()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSCheckMethod.KeySettingsPS, GetDefaultForADTSCheckPS()),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeySettingsPT, GetDefaultForADTSCheckPT())
            };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForADTSCheckPS()
        {
            return new List<ArchivedKeyValuePair>()
            {
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyChannel, CalibChannel.PS),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyPoints, GetDefaultForADTSCheckPSPoints()),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyRate, 50.0),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyUnit, PressureUnits.MBar),
                new ArchivedKeyValuePair(CommonPropertyKeys.KeyEthalons, GetDefaultForADTSEthalonTypes()),
            };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForADTSCheckPT()
        {
            return new List<ArchivedKeyValuePair>()
            {
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyChannel, CalibChannel.PT),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyPoints, GetDefaultForADTSCheckPTPoints()),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyRate, 50.0),
                new ArchivedKeyValuePair(ADTSCheckMethod.KeyUnit, PressureUnits.MBar),
                new ArchivedKeyValuePair(CommonPropertyKeys.KeyEthalons, GetDefaultForADTSEthalonTypes()),
            };
        }

        private static List<ADTSChechPoint> GetDefaultForADTSCheckPSPoints()
        {
            var points = new List<ADTSChechPoint>()
            {
                new ADTSChechPoint(){Pressure = 1100.00, Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 1013.00, Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 843.00,  Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 697.00,  Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 466.00,  Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 189.00,  Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 72.00,   Tolerance = 0.1},
                new ADTSChechPoint(){Pressure = 27.62,   Tolerance = 0.1},
            };
            return points;
        }

        private static List<ADTSChechPoint> GetDefaultForADTSCheckPTPoints()
        {
            var points = new List<ADTSChechPoint>()
            {
                new ADTSChechPoint(){Pressure = 3500.00, Tolerance = 0.49},
                new ADTSChechPoint(){Pressure = 3000.00, Tolerance = 0.46},
                new ADTSChechPoint(){Pressure = 2590.00, Tolerance = 0.40},
                new ADTSChechPoint(){Pressure = 2200.00, Tolerance = 0.36},
                new ADTSChechPoint(){Pressure = 1655.00, Tolerance = 0.32},
                new ADTSChechPoint(){Pressure = 1100.00, Tolerance = 0.28},
                new ADTSChechPoint(){Pressure = 1013.00, Tolerance = 0.27},
                new ADTSChechPoint(){Pressure = 843.00,  Tolerance = 0.27},
                new ADTSChechPoint(){Pressure = 697.00,  Tolerance = 0.26},
                new ADTSChechPoint(){Pressure = 466.00,  Tolerance = 0.25},
                new ADTSChechPoint(){Pressure = 189.00,  Tolerance = 0.25},
                new ADTSChechPoint(){Pressure = 72.00,   Tolerance = 0.24},
                new ADTSChechPoint(){Pressure = 27.62,   Tolerance = 0.24},
            };
            return points;
        }

        public static List<string> GetDefaultForADTSEthalonTypes()
        {
            var ethalons = new List<string>()
            {
                UserEthalonChannel.Key,
                PACE1000Model.Key,
            };
            return ethalons;
        }
        #endregion
    }
}
