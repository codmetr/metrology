using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ADTSChecks.ViewModel.Checks;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks;
using CheckFrame.Model.TransportChannels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Model;
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
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            var method = checkConfig.SelectedMethod;
            if (method is ADTSMethodBase)
            {
                result = ConfigAdtsMethod(method as ADTSMethodBase, checkConfig, checkDeviceChanel, ethalonChanel);
            }
            
            return result;
        }

        IMethodViewModel ConfigAdtsMethod(ADTSMethodBase method, CheckConfig checkConfig, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            method.SetADTS(_deviceManager.GetModel<ADTSModel>());
            method.ChannelType = checkDeviceChanel;
            if (checkConfig.SelectedEthalonTypeKey == UserEthalonChannel.Key)
                method.SetEthalonChannel(null, null);
            else
                method.SetEthalonChannel(_deviceManager.GetEthalonChannel(checkConfig.EthalonDeviceType, ethalonChanel), ethalonChanel);

            if (method is AdtsCheckMethod)
            {
                var adtsMethodic = method as AdtsCheckMethod;
                result = new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSMethodParameters);
            }
            else if (method is ADTSTestMethod)
            {
                var adtsMethodic = method as ADTSTestMethod;
                result = new ADTSTestViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey),
                    _deviceManager, checkConfig.Result, checkConfig.CustomSettings as ADTSMethodParameters);
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
