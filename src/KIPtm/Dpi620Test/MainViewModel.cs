using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using DPI620Genii;
using MahApps.Metro.Controls;
using Moq;
using NLog;
using Tools.View;
using Tools.View.Busy;

namespace Dpi620Test
{
    public class MainViewModel:INotifyPropertyChanged, IBusy
    {
        private readonly LogViewModel _logViewModel;
        private readonly Logger _logger;
        private readonly Dpi620Presenter _dpi620;
        private bool _isConnected;

        public MainViewModel(IDPI620Driver dpi620, SettingsViewModel settings, Dispatcher disp, Action prepareDpi)
        {
            IsBusy = false;
            Settings = settings;
            _logViewModel = new LogViewModel(disp);
            _dpi620 = new Dpi620Presenter(dpi620, prepareDpi);
            _logger = NLog.LogManager.GetLogger("MainLog");
            Slot1 = new SlotViewModel("Слот 1", Settings, dpi620, 1, _logger, this)
            {
                Units = _dpi620.UnitsSlot1,
                SelectedUnit = ""//_dpi620.UnitsSlot1.FirstOrDefault()
            };
            Slot2 = new SlotViewModel("Слот 2", Settings, dpi620, 3, _logger, this)
            {
                Units = _dpi620.UnitsSlot2,
                SelectedUnit = ""//_dpi620.UnitsSlot2.FirstOrDefault()
            };
        }

        public SettingsViewModel Settings { get; }

        /// <summary>
        /// Подключено
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Интерфейс занят
        /// </summary>
        public bool IsBusy { get; set; }

        public ICommand CheckChanged { get
        {
            return new CommandWrapper((arg) =>
            {
                using (new LockControl(this))
                    DoChackChanged(arg);
            });
        } }

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
