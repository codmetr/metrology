using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация DPI620
    /// </summary>
    public class DPI620GeniiConfig : INotifyPropertyChanged
    {
        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfig()
        {
            Slot1 = new DpiSlotConfig() { ChannelType = ChannelType.Pressure };
            Slot2 = new DpiSlotConfig() { ChannelType = ChannelType.Current };
        }
        public IEnumerable<string> Ports { get; set; }

        public string SelectPort
        {
            get { return Properties.Settings.Default.PortName; }
            set
            {
                if(value == Properties.Settings.Default.PortName)
                    return;
                Properties.Settings.Default.PortName = value;
                Properties.Settings.Default.Save();
            }
        }

        public DpiSlotConfig Slot1 { get; set; }

        public DpiSlotConfig Slot2 { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public class DpiSlotConfig:INotifyPropertyChanged
        {
            public DpiSlotConfig()
            {
                ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>();
            }

            private ChannelType _channelType;

            public IEnumerable<ChannelType> ChannelTypes { get; set; }

            public ChannelType ChannelType
            {
                get { return _channelType; }
                set
                {
                    if(value == _channelType)
                        return;
                    _channelType = value;
                    UnitSet = UnitDict.GetUnitsForType(_channelType);
                    SelectedUnit = UnitSet.FirstOrDefault();
                    OnPropertyChanged("ChannelType");
                }
            }

            public double From { get; set; }

            public double To { get; set; }

            public IEnumerable<Units> UnitSet { get; set; }

            public Units SelectedUnit { get; set; }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
    }
}