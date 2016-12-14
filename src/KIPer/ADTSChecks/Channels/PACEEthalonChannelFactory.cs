using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Channels;
using ADTSChecks.Model.Devices;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;

namespace ADTSChecks.Channels
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
