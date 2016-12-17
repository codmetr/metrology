﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;

namespace CheckFrame
{
    public class FeatureDescriptorsCombiner : IFeaturesDescriptor
    {
        private IEnumerable<IFeaturesDescriptor> _features;

        public FeatureDescriptorsCombiner(IEnumerable<IFeaturesDescriptor> features)
        {
            _features = features;
            DeviceTypes = _features.SelectMany(el => el.DeviceTypes);
            EthalonTypes = _features.SelectMany(el => el.EthalonTypes);
            Models = _features.SelectMany(el => el.Models);
            Devices = _features.SelectMany(el => el.Devices);
            ChannelsFactories = _features.SelectMany(el => el.ChannelsFactories);
            EthalonChannels = _features.SelectMany(el => el.EthalonChannels);
        }

        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; private set; }
        public IEnumerable<DeviceTypeDescriptor> EthalonTypes { get; private set; }
        public IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; private set; }
        public IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; private set; }
        public IEnumerable<KeyValuePair<string, IChannelFactory>> ChannelsFactories { get; private set; }
        public IEnumerable<KeyValuePair<string, IEthalonCannelFactory>> EthalonChannels { get; private set; }
        public IEnumerable<ArchivedKeyValuePair> GetDefaultForCheckTypes()
        {
            return _features.SelectMany(el => el.GetDefaultForCheckTypes());
        }
    }
}
