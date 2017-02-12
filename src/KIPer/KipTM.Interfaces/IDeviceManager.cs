using System.Collections.Generic;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.Model
{
    public interface IDeviceManager
    {
        IEthalonChannel GetEthalonChannel(string deviceKey);

        /// <summary>
        /// Получить драйвер устройства
        /// </summary>
        /// <typeparam name="T">Тип драйвера</typeparam>
        /// <param name="transportDescription">Канал связи</param>
        /// <returns></returns>
        T GetDevice<T>(ITransportChannelType transportDescription);
        
        /// <summary>
        /// Получить модель устройства
        /// </summary>
        /// <typeparam name="T">Тип модели</typeparam>
        /// <returns></returns>
        T GetModel<T>();
    }
}