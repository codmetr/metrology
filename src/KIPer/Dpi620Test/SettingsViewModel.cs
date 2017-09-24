using System;
using System.ComponentModel;

namespace Dpi620Test
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public TimeSpan PeriodAutoread { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}