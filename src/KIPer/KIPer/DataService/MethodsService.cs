using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using CheckFrame.Model.Checks;
using KipTM.Interfaces;
using KipTM.Model.Devices;

namespace KipTM.Model
{
    public class MethodsService : IMethodsService
    {
        private readonly Dictionary<string, Dictionary<string, ICheckMethod>> _methods;

        public MethodsService()
        {
            _methods = new Dictionary<string, Dictionary<string, ICheckMethod>>();
            var adtsCheck = new AdtsCheckMethod(NLog.LogManager.GetLogger("ADTSCheckMethod"));
            var adtsTest = new ADTSTestMethod(NLog.LogManager.GetLogger("ADTSTestMethod"));
            _methods.Add(ADTSModel.Key,new Dictionary<string, ICheckMethod>()
            {
                {ADTSTestMethod.Key, adtsTest},
                {AdtsCheckMethod.Key, adtsCheck},
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
