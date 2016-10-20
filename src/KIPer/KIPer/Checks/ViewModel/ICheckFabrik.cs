using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks;
using CheckFrame.Model.TransportChannels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Model;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public interface ICheckFabrik
    {
        /// <summary>
        /// получить презентор типа проверки
        /// </summary>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel);
        
        /// <summary>
        /// получить презентор типа проверки
        /// </summary>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor(
            ICheckMethod method, IEthalonChannel ethalonChannel,
            ITransportChannelType checkDeviceTransport, ITransportChannelType ethalonTransport, 
            IPropertyPool propertyPool, TestResult result, object customSettings);
    }
}