using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Devices;
using KipTM.Settings;

namespace ADTSChecks.Settings
{
    class SettingsFactory : IDeviceSettingsFactory, IEthalonSettingsFactory, IDeviceTypeSettingsFactory
    {
        IEnumerable<DeviceTypeSettings> IDeviceTypeSettingsFactory.GetDefault()
        {
            return new List<DeviceTypeSettings>()
            {
                new DeviceTypeSettings()
                {
                    Key = ADTSModel.Key,
                    Model = ADTSModel.Model,
                    DeviceCommonType = ADTSModel.DeviceCommonType,
                    DeviceManufacturer = ADTSModel.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },
                new DeviceTypeSettings()
                {
                    Key = PACE1000Model.Key,
                    Model = PACE1000Model.Model,
                    DeviceCommonType = PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },

            };
        }

        DeviceSettings IDeviceSettingsFactory.GetDefault()
        {
            return new DeviceSettings()
            {
                Address = "0",
                Name = ADTSModel.Key,
                Model = ADTSModel.Model,
                DeviceCommonType = ADTSModel.DeviceCommonType,
                DeviceManufacturer = ADTSModel.DeviceManufacturer,
                TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                SerialNumber = "123",
                NamePort = "COM2"
            };
        }

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
