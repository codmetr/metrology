using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public class CheckFabrik : ICheckFabrik
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IPropertyPool _propertyPool;

        public CheckFabrik(IDeviceManager deviceManager, IPropertyPool propertyPool)
        {
            _deviceManager = deviceManager;
            _propertyPool = propertyPool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            var method = checkConfig.SelectedCheckType;
            if (method is ADTSCheckMethod)
            {
                var adtsMethodic = method as ADTSCheckMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                adtsMethodic.ChannelType = checkDeviceChanel;
                if (checkConfig.SelectedEthalonTypeKey == UserEthalonChannel.Key)
                    adtsMethodic.SetEthalonChannel(null, null);
                else
                    adtsMethodic.SetEthalonChannel(_deviceManager.GetEthalonChannel(checkConfig.EthalonDeviceType, ethalonChanel), ethalonChanel);
                return new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSMethodParameters);
            }
            else if (method is ADTSTestMethod)
            {
                var adtsMethodic = method as ADTSTestMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                adtsMethodic.ChannelType = checkDeviceChanel;
                if (checkConfig.SelectedEthalonTypeKey == UserEthalonChannel.Key)
                    adtsMethodic.SetEthalonChannel(null, null);
                else
                    adtsMethodic.SetEthalonChannel(_deviceManager.GetEthalonChannel(checkConfig.EthalonDeviceType, ethalonChanel), ethalonChanel);
                return new ADTSTestViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSMethodParameters);
            }
            return null;
        }
    }
}
