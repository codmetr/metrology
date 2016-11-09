using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using ITransportChannelType = KipTM.Model.TransportChannels.ITransportChannelType;

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
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            var method = checkConfig.SelectedMethod;
            if (method is CheckBase)
            {
                result = ConfigAdtsMethod(method as CheckBase, checkConfig, checkDeviceChanel, ethalonChanel);
            }
            
            return result;
        }

        IMethodViewModel ConfigAdtsMethod(CheckBase method, CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            method.SetADTS(_deviceManager.GetModel<ADTSModel>());
            method.ChannelType = checkDeviceChanel;
            if (checkConfig.SelectedEthalonTypeKey == UserEthalonChannel.Key)
                method.SetEthalonChannel(null, null);
            else
                method.SetEthalonChannel(_deviceManager.GetEthalonChannel(checkConfig.EthalonDeviceType, ethalonChanel), ethalonChanel);

            if (method is Calibration)
            {
                var adtsMethodic = method as Calibration;
                result = new CalibrationViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSParameters);
            }
            else if (method is Test)
            {
                var adtsMethodic = method as Test;
                result = new TestViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSParameters);
            }
            if (result != null)
            {
                result.SetEthalonChannel(checkConfig.SelectedEthalonTypeKey, ethalonChanel);
            }
            return result;
        }

        /// <summary>
        /// получить презентор типа проверки
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor( ICheckMethod method, IEthalonChannel ethalonChannel,
            ITransportChannelType checkDeviceTransport, ITransportChannelType ethalonTransport,
            IPropertyPool propertyPool, TestResult result, object customSettings)
        {
            throw new NotImplementedException();
        }
    }
}
