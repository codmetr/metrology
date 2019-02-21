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
    public class DPI620GeniiConfigVm : INotifyPropertyChanged
    {
        private readonly IContext _context;
        private IEnumerable<string> _ports;
        private string _selectPort;

        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfigVm(IContext context)
        {
            _context = context;
            Slot1 = new DpiSlotConfigVm(context) { ChannelType = ChannelType.Pressure };
            Slot2 = new DpiSlotConfigVm(context) { ChannelType = ChannelType.Current };
        }

        public IEnumerable<string> Ports
        {
            get { return _ports; }
            set
            {
                _ports = value; 
                OnPropertyChanged(nameof(Ports));
            }
        }

        public string SelectPort
        {
            get { return _selectPort; }
            set
            {
                _selectPort = value;
                OnPropertyChanged(nameof(SelectPort));
                OnSelectedPortCanged(_selectPort);
            }
        }

        public DpiSlotConfigVm Slot1 { get; set; }

        public DpiSlotConfigVm Slot2 { get; set; }

        public event Action<string> SelectedPortCanged;

        public void SetPortCollection(IEnumerable<string> ports)
        {
            _context.Invoke(()=>Ports = ports);
        }

        public void SetSelectedPort(string port)
        {
            _context.Invoke(() => SelectPort = port);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public class DpiSlotConfigVm:INotifyPropertyChanged
        {
            private readonly IContext _context;
            private ChannelType _channelType;
            private double _from;
            private double _to;
            private Units _selectedUnit;
            private IEnumerable<Units> _unitSet;
            private int _selectedSlotIndex;

            public DpiSlotConfigVm(IContext context)
            {
                _context = context;
                ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>();
            }

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

            public double From
            {
                get { return _from; }
                set { _from = value;
                    OnPropertyChanged("From");
                }
            }

            public double To
            {
                get { return _to; }
                set { _to = value;
                    OnPropertyChanged("To");
                }
            }

            /// <summary>
            /// Варианты единиц измерения
            /// </summary>
            public IEnumerable<Units> UnitSet
            {
                get { return _unitSet; }
                set
                {
                    _unitSet = value;
                    OnPropertyChanged("UnitSet");
                }
            }

            /// <summary>
            /// Выбраные единицы измерения
            /// </summary>
            public Units SelectedUnit
            {
                get { return _selectedUnit; }
                set { _selectedUnit = value;
                    OnPropertyChanged("SelectedUnit");
                }
            }

            /// <summary>
            /// Варианты слотов 
            /// </summary>
            public IEnumerable<int> SlotIndexes { get; private set; }

            /// <summary>
            /// Выбраный слот
            /// </summary>
            public int SelectedSlotIndex
            {
                get { return _selectedSlotIndex; }
                set
                {
                    _selectedSlotIndex = value;
                    OnPropertyChanged("SelectedSlotIndex");
                }
            }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }

        protected virtual void OnSelectedPortCanged(string port)
        {
            SelectedPortCanged?.Invoke(port);
        }
    }
}