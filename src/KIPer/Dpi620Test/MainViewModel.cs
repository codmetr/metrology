using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using DPI620Genii;
using MahApps.Metro.Controls;
using NLog;
using Tools.View;

namespace Dpi620Test
{
    public class MainViewModel:INotifyPropertyChanged
    {
        private readonly LogViewModel _logViewModel;
        private readonly Logger _logger;
        private readonly Dpi620Presenter _dpi620;

        public MainViewModel(IDPI620Driver dpi620, SettingsViewModel settings, Dispatcher disp)
        {
            Settings = settings;
            _logViewModel = new LogViewModel(disp);
            _dpi620 = new Dpi620Presenter(dpi620);
            _logger = NLog.LogManager.GetLogger("MainLog");
            Slot1 = new SlotViewModel("Слот 1", Settings, dpi620, 1, _logger)
            {
                Units = _dpi620.UnitsSlot1, SelectedUnit = _dpi620.UnitsSlot1.FirstOrDefault()
            };
            Slot2 = new SlotViewModel("Слот 2", Settings, dpi620, 2, _logger)
            {
                Units = _dpi620.UnitsSlot2,
                SelectedUnit = _dpi620.UnitsSlot2.FirstOrDefault()
            };
        }

        public SettingsViewModel Settings { get; }

        /// <summary>
        /// Подключено
        /// </summary>
        public bool IsConnected { get; set; }

        public ICommand CheckChanged { get { return new CommandWrapper(DoChackChanged);} }

        private void DoChackChanged(object arg)
        {
            bool isOn = ((ToggleSwitch) arg).IsChecked??false;
            if(isOn)
                _dpi620.Open();
            else
                _dpi620.Close();
            _logger.Trace($"Connected: {(isOn ? "on" : "off")}");
        }

        /// <summary>
        /// Состояние слота номер 1
        /// </summary>
        public SlotViewModel Slot1 { get; set; }

        /// <summary>
        /// Состояние слота номер 2
        /// </summary>
        public SlotViewModel Slot2 { get; set; }

        /// <summary>
        /// Лог
        /// </summary>
        public LogViewModel Log { get { return _logViewModel; } }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        public TimeSpan PeriodAutoread { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

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
    }

    public class OnePointViewModel : INotifyPropertyChanged
    {
        public double Val { get; set; }

        public TimeSpan TimeStamp { get; set; }
            
        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return $"[{TimeStamp}] {Val}";
        }
    }
}
