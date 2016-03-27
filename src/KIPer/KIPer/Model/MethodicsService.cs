using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Model
{
    public class MethodicsService : IMethodicsService
    {
        private readonly Dictionary<string, Dictionary<string, ICheckMethodic>> _methodics;

        public MethodicsService()
        {
            _methodics = new Dictionary<string, Dictionary<string, ICheckMethodic>>();
            //var adtsCheck = new ADTSCheckMethodic(deviceManager.ADTS, NLog.LogManager.GetLogger("ADTSCheckMethodic"));
            //_methodics.Add(ADTSModel.Key, adtsCheck);

        }

        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        public IDictionary<string, ICheckMethodic> MethodicsForType(string DeviceKey)
        {
            return _methodics[DeviceKey];
        }

    }
}
