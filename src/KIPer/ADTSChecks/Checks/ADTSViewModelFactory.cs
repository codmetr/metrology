using System;
using System.Linq;
using ADTSChecks.Checks.Data;
using ADTSChecks.Checks.ViewModel;
using ADTSChecks.Devices;
using ADTSChecks.Model.Checks;
using ArchiveData.DTO;
using CheckFrame.Channels;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.TransportChannels;

namespace ADTSChecks.Checks
{
    [ViewModelFactory(typeof(CheckBaseADTS))]
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
        public ICheckViewModelFactory SetPropertyPool(object propertyPool)
        {
            _propertyPool = propertyPool as IPropertyPool;
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
            TestResultID resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            if(_deviceManager == null)
                throw new NullReferenceException("Not defined _deviceManager");

            if (_propertyPool == null)
                throw new NullReferenceException("Not defined _propertyPool");

            return ConfigAdtsMethod(method as CheckBaseADTS, checkConfig, customSettings as ADTSParameters,
                resultSet, checkDeviceChanel, ethalonChanel);
        }

        /// <summary>
        /// Компановка модели
        /// </summary>
        /// <param name="method"></param>
        /// <param name="checkConfig"></param>
        /// <param name="customSettings"></param>
        /// <param name="resultSet"></param>
        /// <param name="checkDeviceChanel"></param>
        /// <param name="ethalonChanel"></param>
        /// <returns></returns>
        private IMethodViewModel ConfigAdtsMethod(CheckBaseADTS method, CheckConfigData checkConfig, ADTSParameters customSettings,
            TestResultID resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            method.SetADTS(_deviceManager.GetModel<ADTSModel>());
            method.ChConfig.ChannelType = checkDeviceChanel;
            var devType = checkConfig.TargetDevice.Device.DeviceType.TypeKey;
            var ethalonChannel = checkConfig.Ethalons.FirstOrDefault().Value.Channel;
            //if (ethalonChannel.Key == UserEthalonChannel.Channel.Key)
            //    method.SetEthalonChannel(null, null);
            //else
            //    method.SetEthalonChannel(_deviceManager.GetEthalonChannel(ethalonChannel.Key), ethalonChanel);

            if (method is Calibration)
            {
                var adtsMethodic = method as Calibration;
                result = new CalibrationViewModel(adtsMethodic, ADTSCheckConfig.GetDefault(),
                    _deviceManager, resultSet, customSettings);
            }
            else if (method is Test)
            {
                var adtsMethodic = method as Test;
                result = new TestViewModel(adtsMethodic, ADTSCheckConfig.GetDefault(),
                    _deviceManager, resultSet, customSettings);
            }
            //if (ethalonChannel.Key == UserEthalonChannel.Channel.Key)
            //    result.SetEthalonChannel(null, null);
            //else
            //    result.SetEthalonChannel(ethalonChannel.Key, ethalonChanel);
            if (result != null)
            {
                result.SetEthalonChannel(ethalonChannel.Key, ethalonChanel);
            }
            return result;
        }
    }
}
