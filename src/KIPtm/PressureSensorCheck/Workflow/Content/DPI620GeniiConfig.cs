using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Настройка подключения DPI
    /// </summary>
    public class DPI620GeniiConfig
    {
        private readonly DPI620GeniiConfigVm _vm;
        private readonly IDPI620Driver _dpi;

        /// <summary>
        /// Настройка подключения DPI
        /// </summary>
        public DPI620GeniiConfig(DPI620GeniiConfigVm vm, IEnumerable<string> ports, IDPI620Driver dpi)
        {
            _vm = vm;
            _dpi = dpi;
            var settings = Properties.Settings.Default;
            Slot1 = new DpiSlotConfig(_vm.Slot1, settings.Slot1Type, settings.Slot1Index);
            _vm.Slot1.SelectedIndex += i =>
            {   settings.Slot1Index = i;
                settings.Save();};
            _vm.Slot1.SelectedChannel += type =>
            {   settings.Slot1Type = type;
                settings.Save();};
            Slot2 = new DpiSlotConfig(_vm.Slot2, settings.Slot2Type, settings.Slot2Index);
            _vm.Slot2.SelectedIndex += i =>
            {   settings.Slot2Index = i;
                settings.Save();};
            _vm.Slot2.SelectedChannel += type =>
            {   settings.Slot2Type = type;
                settings.Save();};
            var availablePorts = ports.ToArray();
            _vm.SetPortCollection(availablePorts);
            if (availablePorts.Contains(settings.PortName))
                _vm.SetSelectedPort(settings.PortName);
            else
                _vm.SetSelectedPort(availablePorts.FirstOrDefault());
            _vm.SelectedPortCanged += VmOnSelectedPortCanged;
            _vm.RefrashCall += VmOnRefrashCall;
        }

        private void VmOnRefrashCall()
        {
            var indexes = _dpi.TestSlots();
            Slot1.SetSlotIndexes(indexes);
            Slot2.SetSlotIndexes(indexes);
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
        private readonly DpiSlotConfigVm _vm;
        private ChannelType _channelType;
        private double _from;
        private double _to;
        private Units _selectedUnit;
        private IEnumerable<Units> _unitSet;
        private int _selectedSlotIndex;

        public DpiSlotConfig(DpiSlotConfigVm vm, ChannelType type, int index)
        {
            _vm = vm;
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>();
            ChannelType = type;
            _vm.SetChannels(ChannelTypes);
            _vm.SetSelectedChannel(ChannelType);
            _vm.SetUnits(UnitSet);
            _vm.SetSelectedUnit(SelectedUnit);
            _vm.SetSlotIndexes(Enumerable.Range(1, 2));
            _vm.SetSelectedSlotIndex(index);
            _vm.SelectedChannel += VmOnSelectedChannel;
            _vm.SelectedUnut += VmOnSelectedUnut;
            _vm.SelectedIndex += VmOnSelectedIndex;
        }

        private void VmOnSelectedIndex(int slotIndex)
        {
            SelectedSlotIndex = slotIndex;
        }

        private void VmOnSelectedUnut(Units unit)
        {
            SelectedUnit = unit;
        }

        private void VmOnSelectedChannel(ChannelType channelType)
        {
            ChannelType = channelType;
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

        /// <summary>
        /// Обновить набор индексов
        /// </summary>
        /// <param name="indexes"></param>
        public void SetSlotIndexes(IEnumerable<int> indexes)
        {
            _vm.SetSlotIndexes(indexes);
        }
    }
}
