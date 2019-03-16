using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ArchiveData.DTO;
using KipTM.Interfaces;
using Tools.View;

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
            Slot1 = new DpiSlotConfigVm(context) { ChannelType = ChannelType.Pressure, SelectedSlotIndex = 0};
            Slot2 = new DpiSlotConfigVm(context) { ChannelType = ChannelType.Current, SelectedSlotIndex = 1};
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

        public ICommand Refrash { get { return new CommandWrapper(OnRefrashCall); } }

        public DpiSlotConfigVm Slot1 { get; set; }

        public DpiSlotConfigVm Slot2 { get; set; }

        public event Action<string> SelectedPortCanged;

        public event Action RefrashCall;

        public void SetPortCollection(IEnumerable<string> ports)
        {
            _context.Invoke(() => Ports = ports);
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


        protected virtual void OnSelectedPortCanged(string port)
        {
            SelectedPortCanged?.Invoke(port);
        }

        protected virtual void OnRefrashCall()
        {
            RefrashCall?.Invoke();
        }
    }
    public class DpiSlotConfigVm : INotifyPropertyChanged
    {
        private readonly IContext _context;
        private ChannelType _channelType;
        private double _from;
        private double _to;
        private Units _selectedUnit;
        private IEnumerable<Units> _unitSet;
        private int _selectedSlotIndex;
        private IEnumerable<ChannelType> _channelTypes;
        private IEnumerable<int> _slotIndexes;

        public DpiSlotConfigVm(IContext context)
        {
            _context = context;
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>();
        }

        public IEnumerable<ChannelType> ChannelTypes
        {
            get { return _channelTypes; }
            set
            {
                _channelTypes = value;
                OnPropertyChanged(nameof(ChannelTypes));
            }
        }

        public ChannelType ChannelType
        {
            get { return _channelType; }
            set
            {
                _channelType = value;
                OnPropertyChanged(nameof(ChannelType));
                OnSelectedChannel(_channelType);
            }
        }

        public double From
        {
            get { return _from; }
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
            }
        }

        public double To
        {
            get { return _to; }
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
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
                OnPropertyChanged(nameof(UnitSet));
            }
        }

        /// <summary>
        /// Выбраные единицы измерения
        /// </summary>
        public Units SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                _selectedUnit = value;
                OnPropertyChanged(nameof(SelectedUnit));
                OnSelectedUnut(_selectedUnit);
            }
        }

        /// <summary>
        /// Варианты слотов 
        /// </summary>
        public IEnumerable<int> SlotIndexes
        {
            get { return _slotIndexes; }
            set
            {
                _slotIndexes = value;
                OnPropertyChanged(nameof(SlotIndexes));
            }
        }

        /// <summary>
        /// Выбраный слот
        /// </summary>
        public int SelectedSlotIndex
        {
            get { return _selectedSlotIndex; }
            set
            {
                _selectedSlotIndex = value;
                OnPropertyChanged(nameof(SelectedSlotIndex));
                OnSelectedIndex(_selectedSlotIndex);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void SetChannels(IEnumerable<ChannelType> channelTypes)
        {
            _context.Invoke(() => ChannelTypes = channelTypes);
        }

        public void SetSelectedChannel(ChannelType channelType)
        {
            _context.Invoke(() => ChannelType = channelType);
        }

        public event Action<ChannelType> SelectedChannel;

        public void SetUnits(IEnumerable<Units> unitSet)
        {
            _context.Invoke(() => UnitSet = unitSet);
        }

        public void SetSelectedUnit(Units selectedUnit)
        {
            _context.Invoke(() => SelectedUnit = selectedUnit);
        }

        public event Action<Units> SelectedUnut;

        public void SetSlotIndexes(IEnumerable<int> indexes)
        {
            _context.Invoke(() => SlotIndexes = indexes);
        }

        public void SetSelectedSlotIndex(int slotIndex)
        {
            _context.Invoke(() => SelectedSlotIndex = slotIndex);
        }

        public event Action<int> SelectedIndex;

        protected virtual void OnSelectedChannel(ChannelType obj)
        {
            SelectedChannel?.Invoke(obj);
        }

        protected virtual void OnSelectedUnut(Units obj)
        {
            SelectedUnut?.Invoke(obj);
        }

        protected virtual void OnSelectedIndex(int obj)
        {
            SelectedIndex?.Invoke(obj);
        }
    }
}