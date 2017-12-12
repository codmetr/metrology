using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// Сформировать визуальную модель проверки
        /// </summary>
        /// <param name="method">Модель проверки</param>
        /// <param name="checkConfig">Общая конфигурация</param>
        /// <param name="customSettings">Специазизированная настройка</param>
        /// <param name="resultSet">Контейнер результата</param>
        /// <param name="checkDeviceChanel">Канал проверяемого устройства</param>
        /// <param name="ethalonChanel"></param>
        /// <returns></returns>
        IMethodViewModel GetViewModel(object method, CheckConfigData checkConfig, object customSettings,
            TestResultID resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel);
    }
}
