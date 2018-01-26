using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedgraphWpfSimple
{
    public class SignalSin : IObservable<double>, IDisposable, ISignal
    {
        private readonly double _offset = 0.1 * Math.PI;
        private int _step = 0;
        private IObserver<double> _observer;

        public void Handle()
        {
            _observer?.OnNext(GetNext());
        }

        public double GetNext()
        {
            var res = Math.Sin(_step * _offset);
            _step++;
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
