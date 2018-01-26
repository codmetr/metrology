using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedgraphWpfSimple
{
    public class Signal : IObservable<double>, IDisposable, ISignal
    {
        private IObserver<double> _observer;

        public void Handle()
        {
            _observer?.OnNext(GetNext());
        }

        public double GetNext()
        {
            var res = DateTime.Now.Second/60.0;

            return res;
        }

        public IDisposable Subscribe(IObserver<double> observer)
        {
            _observer = observer;
            return this;
        }

        public void Dispose()
        {
            _observer = null;
        }
    }
}
