using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZedgraphWpfSimple
{
    public class Updater:IObserver<double>
    {
        public void Start(ISignal signal, TimeSpan period, CancellationToken cancel)
        {
            while (!cancel.WaitHandle.WaitOne(period))
            {
                signal.Handle();
            }
        }

        public event EventHandler<MeasureEvent> NewMeasure;

        protected virtual void OnNewMeasure(MeasureEvent e)
        {
            NewMeasure?.Invoke(this, e);
        }

        public void OnNext(double value)
        {
            OnNewMeasure(new MeasureEvent(value, DateTime.Now));
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }

    public class MeasureEvent:EventArgs
    {
        public MeasureEvent(double data, DateTime time)
        {
            Data = data;
            Time = time;
        }

        public double Data { get; }

        public DateTime Time { get; }
    }
}
