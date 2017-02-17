using System.Collections.Generic;
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
        IEnumerable<DeviceTypeSettings> IDeviceTypeSettingsFactory.GetDefault()
        {
            return new List<DeviceTypeSettings>()
            {
                new DeviceTypeSettings()
                {
                    Key = PACE1000Model.Key,
                    Model = PACE1000Model.Model,
                    DeviceCommonType = PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                },
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
                Model = PACE1000Model.Model,
                DeviceCommonType = PACE1000Model.DeviceCommonType,
                DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                TypesEtalonParameters = new List<string>(PACE1000Model.TypesEtalonParameters),
                SerialNumber = "123",
                NamePort = "COM1"
            };
        }
    }
}
