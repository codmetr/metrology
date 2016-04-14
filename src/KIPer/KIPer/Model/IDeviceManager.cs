using System.Collections.Generic;
using KipTM.Model.Channels;
using KipTM.Model.Devices;

namespace KipTM.Model
{
    public interface IDeviceManager
    {
        void Init();

        IEthalonChannel GetEthalonChannel(string deviceKey, object settongs);

        PACE5000Model Pace5000 { get; }
        
        ADTSModel ADTS { get; }

        IDictionary<string, IEthalonChannel> EthalonChannels { get; }

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        void StartAutoUpdate();

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        void StopAutoUpdate();
    }
}