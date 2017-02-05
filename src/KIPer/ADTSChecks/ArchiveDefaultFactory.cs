using System.Collections.Generic;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ADTSChecks.Properties;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Model.Channels;
using KipTM.Archive;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces.Archive;

namespace ADTSChecks
{
    public class ArchiveDefaultFactory : IArchiveDataDefault
    {
        #region GetDefault
        public List<ArchivedKeyValuePair> GetDefaultData()
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
                new ArchivedKeyValuePair(Calibration.KeySettingsPS, GetDefaultForADTSCheckPS()),
                new ArchivedKeyValuePair(Calibration.KeySettingsPT, GetDefaultForADTSCheckPT())
            };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForADTSCheckPS()
        {
            return new List<ArchivedKeyValuePair>()
            {
                new ArchivedKeyValuePair(BasicKeys.KeyChannel, new ChannelDescriptor(()=>Resources.ChannelPs)
                    {
                        Name = ADTSModel.Ps,
                        Max = 1355.0,
                        Order = ChannelOrder.Source,
                        TypeChannel = ChannelType.Pressure,
                        Min = 3.0,
                        Error = 0.01,
                    }),
                new ArchivedKeyValuePair(Calibration.KeyPoints, GetDefaultForADTSCheckPSPoints()),
                new ArchivedKeyValuePair(Calibration.KeyRate, 50.0),
                new ArchivedKeyValuePair(Calibration.KeyUnit, PressureUnits.MBar),
                new ArchivedKeyValuePair(CommonPropertyKeys.KeyEthalons, GetDefaultForADTSEthalonTypes()),
            };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForADTSCheckPT()
        {
            return new List<ArchivedKeyValuePair>()
            {
                new ArchivedKeyValuePair(BasicKeys.KeyChannel, 
                    new ChannelDescriptor(()=>Resources.ChannelPt)
                    {
                        Name = ADTSModel.Pt,
                        Max = 2700.0,
                        Order = ChannelOrder.Source,
                        TypeChannel = ChannelType.Pressure,
                        Min = 3.0,
                        Error = 0.01,
                    }),
                new ArchivedKeyValuePair(Calibration.KeyPoints, GetDefaultForADTSCheckPTPoints()),
                new ArchivedKeyValuePair(Calibration.KeyRate, 50.0),
                new ArchivedKeyValuePair(Calibration.KeyUnit, PressureUnits.MBar),
                new ArchivedKeyValuePair(CommonPropertyKeys.KeyEthalons, GetDefaultForADTSEthalonTypes()),
            };
        }

        private static List<ADTSPoint> GetDefaultForADTSCheckPSPoints()
        {
            var points = new List<ADTSPoint>()
            {
                new ADTSPoint(){Pressure = 1100.00, Tolerance = 0.1},
                new ADTSPoint(){Pressure = 1013.00, Tolerance = 0.1},
                new ADTSPoint(){Pressure = 843.00,  Tolerance = 0.1},
                new ADTSPoint(){Pressure = 697.00,  Tolerance = 0.1},
                new ADTSPoint(){Pressure = 466.00,  Tolerance = 0.1},
                new ADTSPoint(){Pressure = 189.00,  Tolerance = 0.1},
                new ADTSPoint(){Pressure = 72.00,   Tolerance = 0.1},
                new ADTSPoint(){Pressure = 27.62,   Tolerance = 0.1},
            };
            return points;
        }

        private static List<ADTSPoint> GetDefaultForADTSCheckPTPoints()
        {
            var points = new List<ADTSPoint>()
            {
                new ADTSPoint(){Pressure = 3500.00, Tolerance = 0.49},
                new ADTSPoint(){Pressure = 3000.00, Tolerance = 0.46},
                new ADTSPoint(){Pressure = 2590.00, Tolerance = 0.40},
                new ADTSPoint(){Pressure = 2200.00, Tolerance = 0.36},
                new ADTSPoint(){Pressure = 1655.00, Tolerance = 0.32},
                new ADTSPoint(){Pressure = 1100.00, Tolerance = 0.28},
                new ADTSPoint(){Pressure = 1013.00, Tolerance = 0.27},
                new ADTSPoint(){Pressure = 843.00,  Tolerance = 0.27},
                new ADTSPoint(){Pressure = 697.00,  Tolerance = 0.26},
                new ADTSPoint(){Pressure = 466.00,  Tolerance = 0.25},
                new ADTSPoint(){Pressure = 189.00,  Tolerance = 0.25},
                new ADTSPoint(){Pressure = 72.00,   Tolerance = 0.24},
                new ADTSPoint(){Pressure = 27.62,   Tolerance = 0.24},
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
