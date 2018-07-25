using System.Collections.Generic;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.Model
{
    public interface IDeviceManager
    {
        /// <summary>
        /// Получить канал по типу
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <returns></returns>
        IEthalonChannel GetEthalonChannel(string deviceKey);

        /// <summary>
        /// Получить визуальную модель для канала
        /// </summary>
        /// <param name="deviceKey">Ключь типа канала</param>
        /// <param name="channel">Канал</param>
        /// <returns></returns>
        object GetEthalonChannelViewModel(string deviceKey, IEthalonChannel channel);

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