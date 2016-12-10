using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Checks;

namespace ADTSChecks
{
    class FeaturesDescriptor : IFeaturesDescriptor
    {
        public FeaturesDescriptor()
        {
            DeviceTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer)
            };
            EthalonTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(PACE1000Model.Model, PACE1000Model.DeviceCommonType, PACE1000Model.DeviceManufacturer)
            };
            Models = new Dictionary<Type, IDeviceModelFactory>()
            {
                {typeof(ADTSModel), new ADTSModelFactory()},
                {typeof(PACE1000Model), new PACE1000ModelFactory()},
            };
            /*TODO
             * Devices = 
             * ChannelsFabrics = 
             * EthalonChannels = 
             */
        }
        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; private set; }
        public IEnumerable<DeviceTypeDescriptor> EthalonTypes { get; private set; }
        public IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; private set; }
        public IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; private set; }
        public IEnumerable<KeyValuePair<string, Func<object, object>>> ChannelsFabrics { get; private set; }
        public IEnumerable<KeyValuePair<string, Func<ITransportChannelType, IEthalonChannel>>> EthalonChannels { get; private set;}

        public IEnumerable<ArchivedKeyValuePair> GetDefaultForCheckTypes()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
                {
                    Calibration.Key,
                }),
            };
        }
    }
}
