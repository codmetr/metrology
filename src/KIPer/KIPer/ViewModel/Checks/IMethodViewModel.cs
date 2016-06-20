using System;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public interface IMethodViewModel
    {
        /// <summary>
        /// Установить канал подключениея для проверяемого прибора
        /// </summary>
        /// <param name="connection"></param>
        void SetConnection(ITransportChannelType connection);

        /// <summary>
        /// Пользовательский канал эталоном 
        /// </summary>
        void SlectUserEthalonChannel();

        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ethalonTypeKey"></param>
        /// <param name="settings"></param>
        void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings);

        /// <summary>
        /// Методика запущена
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Методика остановлена
        /// </summary>
        event EventHandler Stoped;
    }
}