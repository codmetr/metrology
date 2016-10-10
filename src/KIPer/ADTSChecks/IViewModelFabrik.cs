using System;
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
        CheckFrame.ViewModel.Checks.Channels.IMethodViewModel GetViewModelFor(
            CheckFrame.Model.Checks.ICheckMethod methodObj, CheckFrame.Model.Channels.IEthalonChannel ethalonChannel,
            string ethalonTypeKey, CheckFrame.Model.TransportChannels.ITransportChannelType checkDeviceTransport,
            CheckFrame.Model.TransportChannels.ITransportChannelType ethalonTransport, CheckFrame.Model.IDeviceManager deviceManager,
            CheckFrame.Archive.IPropertyPool propertyPool, ArchiveData.DTO.TestResult testResult, object customSettings);
    }
}
