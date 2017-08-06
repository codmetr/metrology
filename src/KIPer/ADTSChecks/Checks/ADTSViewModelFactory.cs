using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Checks;
using ADTSChecks.Checks.Data;
using ADTSChecks.Checks.ViewModel;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    [ViewModelFactoryAttribute(typeof(CheckBase))]
    public class ADTSViewModelFactory : ICheckViewModelFactory
    {
        private IDeviceManager _deviceManager;
        private IPropertyPool _propertyPool;

        //public ADTSViewModelFactory(IDeviceManager deviceManager, IPropertyPool propertyPool)
        //{
        //    _deviceManager = deviceManager;
        //    _propertyPool = propertyPool;
        //}

        /// <summary>
        /// Сконфигурировать набор драйверов устройств
        /// </summary>
        public ICheckViewModelFactory SetDeviceManager(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            return this;
        }

        /// <summary>
        /// Сконфигурировать пул свойств
        /// </summary>
        public ICheckViewModelFactory SetPropertyPool(IPropertyPool propertyPool)
        {
            _propertyPool = propertyPool;
            return this;
        }

        /// <summary>
        /// Получить визуальную модель проверки
        /// </summary>
        /// <param name="method"></param>
        /// <param name="checkConfig"></param>
        /// <param name="customSettings"></param>
        /// <param name="resultSet"></param>
        /// <param name="checkDeviceChanel"></param>
        /// <param name="ethalonChanel"></param>
        /// <returns></returns>
        public IMethodViewModel GetViewModel(object method, CheckConfigData checkConfig, object customSettings,
            TestResult resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            if(_deviceManager == null)
                throw new NullReferenceException("Not defined _deviceManager");

            if (_propertyPool == null)
                throw new NullReferenceException("Not defined _propertyPool");

            return ConfigAdtsMethod(method as CheckBase, checkConfig, customSettings as ADTSParameters,
                resultSet, checkDeviceChanel, ethalonChanel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="checkConfig"></param>
        /// <param name="customSettings"></param>
        /// <param name="resultSet"></param>
        /// <param name="checkDeviceChanel"></param>
        /// <param name="ethalonChanel"></param>
        /// <returns></returns>
        IMethodViewModel ConfigAdtsMethod(CheckBase method, CheckConfigData checkConfig, ADTSParameters customSettings,
            TestResult resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            method.SetADTS(_deviceManager.GetModel<ADTSModel>());
            method.ChannelType = checkDeviceChanel;
            if (checkConfig.EthalonTypeKey == UserEthalonChannel.Key)
                method.SetEthalonChannel(null, null);
            else
                method.SetEthalonChannel(_deviceManager.GetEthalonChannel(checkConfig.EthalonTypeKey), ethalonChanel);

            if (method is Calibration)
            {
                var adtsMethodic = method as Calibration;
                result = new CalibrationViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.TargetTypeKey),
                    _deviceManager, resultSet, customSettings);
            }
            else if (method is Test)
            {
                var adtsMethodic = method as Test;
                result = new TestViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.TargetTypeKey),
                    _deviceManager, resultSet, customSettings);
            }
            if (result != null)
            {
                result.SetEthalonChannel(checkConfig.EthalonTypeKey, ethalonChanel);
            }
            return result;
        }
    }
}
