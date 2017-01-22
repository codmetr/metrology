using System.Collections.Generic;
using System.Linq;
using KipTM.Interfaces;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    public class MethodsService : IMethodsService
    {
        private readonly Dictionary<string, Dictionary<string, ICheckMethod>> _methods;

        public MethodsService(IEnumerable<IMethodFactory> factories )
        {
            _methods = factories.Select(el => el.GetMethod()).ToDictionary(el => el.Item1, el => el.Item2);
            //_methods = new Dictionary<string, Dictionary<string, ICheckMethod>>();
            //var adtsCheck = new Calibration(NLog.LogManager.GetLogger("ADTSCheckMethod"));
            //var adtsTest = new Test(NLog.LogManager.GetLogger("ADTSTestMethod"));
            //_methods.Add(ADTSModel.Key,new Dictionary<string, ICheckMethod>()
            //{
            //    {Test.Key, adtsTest},
            //    {Calibration.Key, adtsCheck},
            //}); 

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
