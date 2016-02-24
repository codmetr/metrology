using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MainLoop;
using PACESeries;

namespace KipTM.Model.Devices
{
    public class PACE5000Model
    {
        private PACESeries.PACEDriver _driver;
        private ILoops _loops;
        private string _loopKey;
        public PACE5000Model(string title, ILoops loops, string loopKey, PACEDriver driver)
        {
            Title = title;
            _loopKey = loopKey;
            _driver = driver;
        }

        public string Title
        {
            get;
            private set;
        }

        internal static string Key{get { return "PACE5000"; }}

        public void SetAutoread(TimeSpan autoreadPeriod)
        {
            
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
