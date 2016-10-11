﻿using System;
using CheckFrame.Model;
using CheckFrame.ViewModel.Checks.Channels;
using CheckFrame.Model.Checks;
using CheckFrame.Model.Channels;
using CheckFrame.Model.TransportChannels;
using ArchiveData.DTO;
using CheckFrame.Archive;
namespace ADTSChecks
{
    interface IViewModelFabrik
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodObj"></param>
        /// <param name="ethalonChannel"></param>
        /// <param name="ethalonTypeKey"></param>
        /// <param name="checkDeviceTransport"></param>
        /// <param name="ethalonTransport"></param>
        /// <param name="deviceManager"></param>
        /// <param name="propertyPool"></param>
        /// <param name="testResult"></param>
        /// <param name="customSettings"></param>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor( ICheckMethod methodObj, IEthalonChannel ethalonChannel,
            string ethalonTypeKey, ITransportChannelType checkDeviceTransport, ITransportChannelType ethalonTransport,
            IDeviceManager deviceManager, IPropertyPool propertyPool, TestResult testResult, object customSettings);
    }
}
