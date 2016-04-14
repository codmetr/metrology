using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MainLoop;
using PACESeries;

namespace KipTM.Model.Devices
{
    public class PACE1000Model
    {
        private readonly PACESeries.PACEDriver _driver;
        private ILoops _loops;
        private string _loopKey;
        public PACE1000Model(string title, ILoops loops, string loopKey, PACEDriver driver)
        {
            Title = title;
            _loops = loops;
            _loopKey = loopKey;
            _driver = driver;
        }

        public string Title
        {
            get;
            private set;
        }

        internal static string Key{get { return "PACE5000"; }}
        internal static string Model { get { return "PACE5000"; } }
        internal static string DeviceCommonType { get { return "Калибратор давления"; } }
        internal static string DeviceManufacturer { get { return "GE Druk"; } }
        internal static IEnumerable<string> TypesEtalonParameters = new[] { "давление", "авиационная высота", "авиационная скорость" };

        public void SetAutoread(TimeSpan autoreadPeriod)
        {
            throw new NotImplementedException();
        }

        public double GetPressure()
        {
            throw new NotImplementedException();
        }

        private void AutoreadFunction(object parameters)
        {
            var paramTuple = parameters as Tuple<TimeSpan, CancellationToken, ILoops, string>;
            if(paramTuple == null)
                return;
            var period = paramTuple.Item1;
            var cancel = paramTuple.Item2;
            var loops = paramTuple.Item3;
            var loopKey = paramTuple.Item4;

            while (!cancel.IsCancellationRequested)
            {
                loops.StartMiddleAction(loopKey, (mb) => _driver.GetAltetude());
            }
        }
    }
}
