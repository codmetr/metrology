using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public interface ICheckFabrik
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel);
    }
}