using System;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using PACEChecks.Channels.ViewModel;
using PACEChecks.Devices;

namespace PACEChecks.Channels
{
    /// <summary>
    /// Фабрика эталонного канала PACE1000
    /// </summary>
    public class PACEEthalonChannelFactory : IEthalonCannelFactory
    {
        /// <summary>
        /// Получить эталонный канал PACE1000
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEthalonChannel GetChanel(object model)
        {
            return new PACEEthalonChannel(model as PACE1000Model);
        }

        /// <summary>
        /// Получить визуальную модель заданного эталонного канала полученного из <see cref="GetChanel"/>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object GetChanelViewModel(IEthalonChannel channel)
        {
            if (channel is PACEEthalonChannel)
            {
                return new PaceEthalonChannelViewModel(channel as PACEEthalonChannel);
            }
            return null;
        }

        /// <summary>
        /// Требуемый тип модели PACE1000Model
        /// </summary>
        public Type ModelType { get { return typeof (PACE1000Model); } }
    }
}
