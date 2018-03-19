using System;
using System.Collections.Generic;
using System.Threading;
using DPI620Genii;
using NLog;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    public class AutoUpdater : IObservable<MeasuringPoint>
    {

        internal class AutoreadState
        {
            public AutoreadState(CancellationToken cancel, TimeSpan periodRepeat, DateTime startTime, EventWaitHandle autoreadWh)
            {
                Cancel = cancel;
                PeriodRepeat = periodRepeat;
                StartTime = startTime;
                AutoreadWh = autoreadWh;
            }

            public CancellationToken Cancel { get; }

            public TimeSpan PeriodRepeat { get; }

            public DateTime StartTime { get; }

            public EventWaitHandle AutoreadWh { get; }
        }

        public AutoUpdater(Logger logger)
        {
            _logger = logger;
            observers = new List<IObserver<MeasuringPoint>>();
        }

        private readonly Logger _logger;
        private List<IObserver<MeasuringPoint>> observers;

        public IDisposable Subscribe(IObserver<MeasuringPoint> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        internal void Start(DPI620DriverCom dpi620, AutoreadState arg, PressureSensorConfig conf)
        {
            try
            {
                while (!arg.Cancel.WaitHandle.WaitOne(arg.PeriodRepeat))
                {
                    var outVal = dpi620.GetValue(1);
                    var pressure = dpi620.GetValue(2);
                    var outPoint = GetOutForPressure(conf, pressure);
                    var dOut = outPoint - outVal;
                    var qu = outVal / outPoint - 1.0;
                    var item = new MeasuringPoint()
                    {
                        TimeStamp = DateTime.Now - arg.StartTime,
                        I = outVal,
                        Pressure = pressure,
                        dI = dOut,
                        In = GetOutForPressure(conf, pressure),
                        dIn = GetdOutForOut(conf, outPoint),
                        qI = qu,
                        qIn = 0,
                    };
                    Publish(item);
                    _logger.Trace($"Readed repeat: P:{item.Pressure}");
                }
            }
            finally
            {
                arg.AutoreadWh.Set();
            }
        }


        private double GetOutForPressure(PressureSensorConfig conf, double pressure)
        {
            var percentVpi = (pressure - conf.VpiMin) / (conf.VpiMax - conf.VpiMin);
            if (pressure < conf.VpiMin)
                percentVpi = 0;
            double outMin = 0.0;
            double outMax = 5.0;
            if (conf.OutputRange == OutGange.I4_20mA)
            {
                outMin = 4;
                outMax = 20;
            }
            var val = outMin + (outMax - outMin) * percentVpi;

            return val;
        }

        /// <summary>
        /// Получить допуск для конкретной точки напряжения
        /// </summary>
        /// <param name="outVal"></param>
        /// <returns></returns>
        private double GetdOutForOut(PressureSensorConfig conf, double outVal)
        {
            return conf.ToleranceDelta;
        }

        private void Publish(MeasuringPoint data)
        {
            if (observers == null)
                return;
            foreach (var observer in observers)
            {
                observer.OnNext(data);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<MeasuringPoint>> _observers;
            private IObserver<MeasuringPoint> _observer;

            public Unsubscriber(List<IObserver<MeasuringPoint>> observers, IObserver<MeasuringPoint> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}