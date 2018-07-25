using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTS;
using KipTM.Interfaces.Channels;

namespace VisaChannel
{
    /// <summary>
    /// фабрика заглушки для канала VISA
    /// </summary>
    class VisaFakeFactory : IDeviceConfig
    {
        /// <summary>
        /// Заглушка для канала VISA
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public object GetDriver(object opt)
        {
            return new FakeTransport();
        }
    }
}
