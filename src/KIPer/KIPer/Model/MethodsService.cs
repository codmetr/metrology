using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Model
{
    public class MethodsService : IMethodsService
    {
        private readonly Dictionary<string, Dictionary<string, ICheckMethod>> _methods;

        public MethodsService()
        {
            _methods = new Dictionary<string, Dictionary<string, ICheckMethod>>();
            var adtsCheck = new ADTSCheckMethod(NLog.LogManager.GetLogger("ADTSCheckMethod"));
            var adtsTest = new ADTSTestMethod(NLog.LogManager.GetLogger("ADTSTestMethod"));
            _methods.Add(ADTSModel.Key,new Dictionary<string, ICheckMethod>()
            {
                {ADTSCheckMethod.Key, adtsCheck},
                {ADTSTestMethod.Key, adtsTest},
            }); 

        }

        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        public IDictionary<string, ICheckMethod> MethodsForType(string DeviceKey)
        {
            return _methods[DeviceKey];
        }

    }
}
