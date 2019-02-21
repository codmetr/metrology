using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Настройка подключения DPI
    /// </summary>
    class DPI620GeniiConfig
    {
        private DPI620GeniiConfigVm _vm;
        private readonly IEnumerable<string> _ports;

        public DPI620GeniiConfig(DPI620GeniiConfigVm vm, IEnumerable<string> ports)
        {
            _vm = vm;
            _ports = ports;
            Slot1 = new DpiSlotConfig(_vm.Slot1);
            Slot2 = new DpiSlotConfig(_vm.Slot2);
            _vm.SetPortCollection(_ports);
            _vm.SetSelectedPort(Properties.Settings.Default.PortName);
            _vm.SelectedPortCanged += VmOnSelectedPortCanged;
        }

        private void VmOnSelectedPortCanged(string port)
        {
            SelectPort = port;
        }

        public string SelectPort
        {
            get { return Properties.Settings.Default.PortName; }
            set
            {
                if (value == Properties.Settings.Default.PortName)
                    return;
                Properties.Settings.Default.PortName = value;
                Properties.Settings.Default.Save();
            }
        }

        public DpiSlotConfig Slot1 { get; set; }

        public DpiSlotConfig Slot2 { get; set; }
    }

    /// <summary>
    /// Настройка слота DPI
    /// </summary>
    public class DpiSlotConfig
    {
        private DPI620GeniiConfigVm.DpiSlotConfigVm _vm;
        private ChannelType _channelType;
        private double _from;
        private double _to;
        private Units _selectedUnit;
        private IEnumerable<Units> _unitSet;
        private int _selectedSlotIndex;

        public DpiSlotConfig(DPI620GeniiConfigVm.DpiSlotConfigVm vm)
        {
            _vm = vm;
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>();
            SlotIndexes = Enumerable.Range(0, 3);
        }

        public IEnumerable<ChannelType> ChannelTypes { get; set; }

        public ChannelType ChannelType
        {
            get { return _channelType; }
            set
            {
                if (value == _channelType)
                    return;
                _channelType = value;
                UnitSet = UnitDict.GetUnitsForType(_channelType);
                SelectedUnit = UnitSet.FirstOrDefault();
            }
        }

        public double From
        {
            get { return _from; }
            set
            {
                _from = value;
            }
        }

        public double To
        {
            get { return _to; }
            set
            {
                _to = value;
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
            }
        }
    }
}
