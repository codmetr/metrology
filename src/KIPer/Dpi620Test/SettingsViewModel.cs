using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dpi620Test
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public TimeSpan PeriodAutoread { get; set; }

        public IEnumerable<string> Ports { get; set; }

        public string SelectedPort { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}