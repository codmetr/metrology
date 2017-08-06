using System;
using CheckFrame.Model;
using CheckFrame.ViewModel.Checks.Channels;
using CheckFrame.Model.Checks;
using CheckFrame.Model.Channels;
using ArchiveData.DTO;
using KipTM.Archive;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;

namespace ADTSChecks
{
    interface IViewModelFactory
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
