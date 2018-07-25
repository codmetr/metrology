using System;
using ArchiveData.DTO;
using KipTM.EventAggregator;
using KipTM.Model.TransportChannels;

namespace CheckFrame.ViewModel.Checks.Channels
{
    public interface IMethodViewModel
    {
        void SetAggregator(IEventAggregator agregator);

        /// <summary>
        /// Установить канал подключениея для проверяемого прибора
        /// </summary>
        /// <param name="connection"></param>
        void SetConnection(ITransportChannelType connection);

        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ethalonTypeKey"></param>
        /// <param name="settings"></param>
        void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings);

        /// <summary>
        /// Текущий результат
        /// </summary>
        TestResultID CurrentResult { get; }
        
        /// <summary>
        /// Методика запущена
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Методика остановлена
        /// </summary>
        event EventHandler Stoped;

        /// <summary>
        /// Отчищение состояний
        /// </summary>
        void Cleanup();
    }
}