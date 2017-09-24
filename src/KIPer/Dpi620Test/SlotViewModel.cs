using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DPI620Genii;
using NLog;
using Tools.View;

namespace Dpi620Test
{
    public class SlotViewModel : INotifyPropertyChanged
    {
        private readonly Logger _logger;

        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        private DateTime? _startTime = null;

        private readonly ManualResetEvent _autorepeatWh = new ManualResetEvent(true);

        private readonly SettingsViewModel _settings;

        private readonly IDPI620Driver _dpi620;

        private readonly int _slotNum;

        public SlotViewModel(string name, SettingsViewModel settings, IDPI620Driver dpi620, int slotNum, Logger logger)
        {
            Name = name;
            _settings = settings;
            _dpi620 = dpi620;
            _slotNum = slotNum;
            _logger = logger;
            ReadedPoints = new ObservableCollection<OnePointViewModel>(new List<OnePointViewModel>()
            {
                new OnePointViewModel() { TimeStamp = TimeSpan.FromMilliseconds(0), Val = 1},
                new OnePointViewModel() { TimeStamp = TimeSpan.FromMilliseconds(10), Val = 1.5},
                new OnePointViewModel() { TimeStamp = TimeSpan.FromMilliseconds(20), Val = 1.9},
                new OnePointViewModel() { TimeStamp = TimeSpan.FromMilliseconds(30), Val = 10},
            });

        }

        public string Name { get; set; }

        public IEnumerable<string> Units { get; set; }

        public string SelectedUnit { get; set; }

        public bool IsAutorequest { get; set; }

        public ICommand SetUnit => new CommandWrapper(DoSetUnit);

        private void DoSetUnit()
        {
            _dpi620.SetUnits(_slotNum, SelectedUnit);
        }

        public ICommand ReadOnce =>new CommandWrapper(DoReadOnce);

        private void DoReadOnce()
        {
            var val = _dpi620.GetValue(_slotNum, SelectedUnit);
            ReadingResult = $"{val} {SelectedUnit}";
            _logger.Trace($"{Name} readed once: {ReadingResult}");
        }

        public ICommand StartAutoread => new CommandWrapper(DoStartAutoread);

        private void DoStartAutoread()
        {
            if(_startTime!=null)
                return;
            _startTime = DateTime.Now;
            var cancel = _cancellation.Token;
            _autorepeatWh.Reset();
            ReadedPoints.Clear();
            Task.Factory.StartNew((arg) => Autoread((AutoreadState) arg),
                new AutoreadState(cancel, _settings.PeriodAutoread, _startTime.Value, _autorepeatWh),
                cancel);
        }

        public ICommand StopAutoread => new CommandWrapper(DoStopAutoread);

        private void DoStopAutoread()
        {
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
            _autorepeatWh.WaitOne();
            _startTime = null;
        }

        private void Autoread(AutoreadState arg)
        {
            while (!arg.Cancel.WaitHandle.WaitOne(arg.PeriodRepeat))
            {
                var item = new OnePointViewModel()
                {
                    TimeStamp = DateTime.Now - arg.StartTime,
                    Val = _dpi620.GetValue(_slotNum, SelectedUnit)
                };
                ReadedPoints.Add(item);
                _logger.Trace($"{Name} readed repeat: {item} {SelectedUnit}");
            }
            arg.AutoreadWh.Set();
        }

        public string ReadingResult { get; set; }

        public ObservableCollection<OnePointViewModel> ReadedPoints { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        protected class AutoreadState
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
    }
}