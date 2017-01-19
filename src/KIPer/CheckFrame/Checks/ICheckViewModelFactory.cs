using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Model;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Checks
{
    /// <summary>
    /// Формирователь визуальной модели для метода проверки
    /// </summary>
    public interface ICheckViewModelFactory
    {
        /// <summary>
        /// Сконфигурировать набор драйверов устройств
        /// </summary>
        ICheckViewModelFactory SetDeviceManager(IDeviceManager deviceManager);

        /// <summary>
        /// Сконфигурировать пул свойств
        /// </summary>
        ICheckViewModelFactory SetPropertyPool(IPropertyPool propertyPool);

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
        IMethodViewModel GetViewModel(object method, CheckConfigData checkConfig, object customSettings,
            TestResult resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel);
    }
}
