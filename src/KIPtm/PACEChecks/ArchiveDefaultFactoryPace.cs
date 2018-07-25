using System.Collections.Generic;
using ArchiveData.DTO;
using CheckFrame.Archive;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces.Archive;
using PACEChecks.Devices;
using PACEChecks.Properties;

namespace PACEChecks
{
    /// <summary>
    /// Фабрика значений по умолчанию для PACE
    /// </summary>
    public class ArchiveDefaultFactoryPace : IArchiveDataDefault
    {
        #region GetDefault
        /// <summary>
        /// Ключ: "тип объекта контроля" 
        /// </summary>
        /// <returns></returns>
        public List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(PACE1000Model.Key, GetDefaultForADTS()),
            };
        }

        /// <summary>
        /// Ключ: "тип измерительного канала" 
        /// </summary>
        /// <returns></returns>
        public static List<ArchivedKeyValuePair> GetDefaultForADTS()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(PACEData.KeysDic.PACE1000Pressure, GetDefaultForPACE1000Pressure()),
            };
        }

        /// <summary>
        /// Ключ: "тип характеристики канала"
        /// </summary>
        /// <returns></returns>
        private static List<ArchivedKeyValuePair> GetDefaultForPACE1000Pressure()
        {
            return new List<ArchivedKeyValuePair>()
            {
                new ArchivedKeyValuePair(BasicKeys.KeyChannel, new ChannelDescriptor()
                    {
                        Key = BasicKeys.KeyChannel,
                        Name = PACEData.KeysDic.PACE1000Pressure,
                        Max = 1355.0,
                        Order = ChannelOrder.Measuring,
                        TypeChannel = ChannelType.Pressure,
                        Min = 3.0,
                        Error = 0.01,
                    }.SetLocalizedNameFunc(()=>Resource.ChannelPressure)),
            };
        }

        #endregion
    }
}
