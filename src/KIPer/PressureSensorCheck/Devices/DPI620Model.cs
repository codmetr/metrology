using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.Model;

namespace PressureSensorCheck.Devices
{
    /// <summary>
    /// Модель DPI620Genii
    /// </summary>
    public class DPI620Model
    {
        /// <summary>
        /// Описатель типа DPI620Genii
        /// </summary>
        public static DeviceTypeDescriptor Descriptor =>
            new DeviceTypeDescriptor(Model, DeviceCommonType, DeviceManufacturer)
            {
                TypeKey = Key,
                Function = DeviceTypeDescriptor.FunctionType.Controller,
            };
        public static string Key { get { return "DPI620Genii"; } }
        public static string Model { get { return "DPI620Genii"; } }
        public static string DeviceCommonType { get { return "Переносное устройство калибровки давления"; } }
        public static string DeviceManufacturer { get { return "GE"; } }

        public enum Unit
        {
            kgSm,
            volt,
        }

        private Dictionary<Unit, string> _unitDict = new Dictionary<Unit, string>()
        { // todo: выяснить настоящие коды (указаны коды для примера)
            {Unit.kgSm, "kgsm" },
            {Unit.volt, "volt" },
        };

        private Dictionary<int, Unit> _curUnit = new Dictionary<int, Unit>()
        {
            {0, Unit.kgSm },
            {1, Unit.volt },
        };
        private IDeviceManager _deviceManager;
        private DPI620DriverUsb _dpi620;

        public DPI620Model(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }


        public void Open()
        {
            _dpi620 = _deviceManager.GetDevice<DPI620DriverUsb>(null);
            _dpi620.Open();
        }

        public void Close()
        {
            _dpi620.Close();
        }

        public void SetUnit(int slot, Unit unit)
        {
            _dpi620.SetUnits(slot, _unitDict[unit]);
        }

        public double GetValue(int slot)
        {
            var unit = _unitDict[_curUnit[slot]];
            var res = _dpi620.GetValue(slot, unit);
            return res;
        }
    }
}
