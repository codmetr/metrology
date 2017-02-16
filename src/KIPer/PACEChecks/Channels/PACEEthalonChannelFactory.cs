using System;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
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
        /// Требуемый тип модели PACE1000Model
        /// </summary>
        public Type ModelType { get { return typeof (PACE1000Model); } }
    }
}
