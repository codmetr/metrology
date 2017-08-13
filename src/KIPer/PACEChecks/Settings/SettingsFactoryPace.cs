using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Interfaces.Settings;
using KipTM.Settings;
using PACEChecks.Devices;

namespace PACEChecks.Settings
{
    public class SettingsFactoryPace : /*IDeviceSettingsFactory,*/ IEthalonSettingsFactory, IDeviceTypeSettingsFactory
    {
        /// <summary>
        /// Типы проверяемых устройств и их измерительные каналы
        /// </summary>
        /// <returns>Набор описателей устройств</returns>
        IEnumerable<DeviceTypeDescriptor> IDeviceTypeSettingsFactory.GetDefault()
        {
            return new List<DeviceTypeDescriptor>()
            {
                PACE1000Model.Descriptor,
                //new DeviceTypeSettings()
                //{
                //    Key = PACE1000Model.Key,
                //    Model = PACE1000Model.Model,
                //    DeviceCommonType = PACE1000Model.DeviceCommonType,
                //    DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                //},
            };
        }

        /// <summary>
        /// Настройки подключения эталона по умолчанию
        /// </summary>
        /// <returns></returns>
        DeviceSettings IEthalonSettingsFactory.GetDefault()
        {
            return new DeviceSettings()
            {
                Address = "0",
                Name = PACE1000Model.Key,
                TypeDescriptor = PACE1000Model.Descriptor,
                //Model = PACE1000Model.Model,
                //DeviceCommonType = PACE1000Model.DeviceCommonType,
                //DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                TypesEtalonParameters = new List<string>(PACE1000Model.TypesEtalonParameters),
                SerialNumber = "123",
                NamePort = "COM1"
            };
        }
    }
}
