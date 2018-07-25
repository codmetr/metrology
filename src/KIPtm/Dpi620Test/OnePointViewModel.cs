using System;
using System.ComponentModel;

namespace Dpi620Test
{
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