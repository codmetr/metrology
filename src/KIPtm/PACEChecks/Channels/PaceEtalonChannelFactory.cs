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
    public class PaceEtalonChannelFactory : IEtalonCannelFactory
    {
        /// <summary>
        /// Получить эталонный канал PACE1000
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEtalonChannel GetChanel(object model)
        {
            return new PaceEtalonChannel(model as PACE1000Model);
        }

        /// <summary>
        /// Получить визуальную модель заданного эталонного канала полученного из <see cref="GetChanel"/>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object GetChanelViewModel(IEtalonChannel channel)
        {
            if (channel is PaceEtalonChannel)
            {
                return new PaceEthalonChannelViewModel(channel as PaceEtalonChannel);
            }
            return null;
        }

        /// <summary>
        /// Требуемый тип модели PACE1000Model
        /// </summary>
        public Type ModelType { get { return typeof (PACE1000Model); } }
    }
}
