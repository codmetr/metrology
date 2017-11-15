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

        private IDeviceManager _deviceManager;
        private IDPI620Driver _dpi620;

        public DPI620Model(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }


        public void Open(string port)
        {
            var dpi620 = _deviceManager.GetDevice<DPI620DriverCom>(null);
            dpi620.SetPort(port);
            dpi620.Open();
            _dpi620 = dpi620;
        }

        public void Close()
        {
            _dpi620.Close();
        }

        public double GetValue(int slot)
        {
            var res = _dpi620.GetValue(slot);
            return res;
        }
    }
}
