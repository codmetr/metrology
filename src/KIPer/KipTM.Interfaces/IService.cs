using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces
{
    /// <summary>
    /// Универсальное управление сервисом
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Название сервиса
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="channel">Канал для работы сервиса</param>
        void Start(ITransportChannelType channel);

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        void Stop();
    }
}
