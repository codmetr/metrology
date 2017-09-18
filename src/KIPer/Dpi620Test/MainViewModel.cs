using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.View;

namespace Dpi620Test
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public SettingsViewModel Settings { get; set; }

        /// <summary>
        /// Подключено
        /// </summary>
        public bool IsConnected { get; set; }

        public SlotViewModel Slot1 { get; set; }

        public SlotViewModel Slot2 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        public TimeSpan PeriodAutoread { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SlotViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public IEnumerable<string> Units { get; set; }

        public string SelectedUnit { get; set; }

        public bool IsAutorequest { get; set; }

        public ICommand SetUnit => new CommandWrapper(DoSetUnit);

        private void DoSetUnit()
        {
        }

        public ICommand ReadOnce =>new CommandWrapper(DoReadOnce);

        private void DoReadOnce()
        {
        }

        public ICommand StartAutoread => new CommandWrapper(DoStartAutoread);

        private void DoStartAutoread()
        {
        }

        public ICommand StopAutoread => new CommandWrapper(DoStopAutoread);

        private void DoStopAutoread()
        {
        }

        public string ReadingResult { get; set; }

        public ObservableCollection<OnePointViewModel> ReadedPoints { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class OnePointViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
