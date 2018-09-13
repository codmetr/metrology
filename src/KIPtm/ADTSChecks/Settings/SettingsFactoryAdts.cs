using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Devices;
using ArchiveData.DTO;
using KipTM.Interfaces.Settings;
using KipTM.Settings;

namespace ADTSChecks.Settings
{
    public class SettingsFactoryAdts : IDeviceSettingsFactory, /*IEtalonSettingsFactory,*/ IDeviceTypeSettingsFactory
    {
        /// <summary>
        /// Типы проверяемых устройств и их измерительные каналы
        /// </summary>
        /// <returns>Набор описателей устройств</returns>
        IEnumerable<DeviceTypeDescriptor> IDeviceTypeSettingsFactory.GetDefault()
        {
            return new List<DeviceTypeDescriptor>()
            {
                ADTSModel.Descriptor,
                //new DeviceTypeSettings()
                //{
                //    Key = ADTSModel.Key,
                //    Model = ADTSModel.Model,
                //    DeviceCommonType = ADTSModel.DeviceCommonType,
                //    DeviceManufacturer = ADTSModel.DeviceManufacturer,
                //},
            };
        }

        /// <summary>
        /// Настройки подключения объекта по умолчанию
        /// </summary>
        /// <returns></returns>
        DeviceSettings IDeviceSettingsFactory.GetDefault()
        {
            return new DeviceSettings()
            {
                Address = "0",
                Name = ADTSModel.Key,
                TypeDescriptor = ADTSModel.Descriptor,
                //Model = ADTSModel.Model,
                //DeviceCommonType = ADTSModel.DeviceCommonType,
                //DeviceManufacturer = ADTSModel.DeviceManufacturer,
                TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                SerialNumber = "123",
                NamePort = "COM2"
            };
        }

        ///// <summary>
        ///// Настройки подключения эталона по умолчанию
        ///// </summary>
        ///// <returns></returns>
        //DeviceSettings IEtalonSettingsFactory.GetDefault()
        //{
        //    return new DeviceSettings()
        //    {
        //        Address = "0",
        //        Name = PACE1000Model.Key,
        //        Model = PACE1000Model.Model,
        //        DeviceCommonType = PACE1000Model.DeviceCommonType,
        //        DeviceManufacturer = PACE1000Model.DeviceManufacturer,
        //        TypesEtalonParameters = new List<string>(PACE1000Model.TypesEtalonParameters),
        //        SerialNumber = "123",
        //        NamePort = "COM1"
        //    };
        //}
    }
}
