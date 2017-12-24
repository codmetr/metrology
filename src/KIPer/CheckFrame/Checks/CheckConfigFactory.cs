using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Interfaces;
using KipTM.Interfaces.Settings;
using KipTM.Settings;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Фабрика конфигурации проверки
    /// </summary>
    public class CheckConfigFactory
    {
        /// <summary>
        /// Сгенерировать конфигурацию для заданного типа прибота
        /// </summary>
        /// <param name="key"></param>
        /// <param name="settings"></param>
        /// <param name="method"></param>
        /// <param name="propertyPool"></param>
        /// <param name="dictionaries"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static CheckConfigDevice GenerateForType(
            DeviceTypeDescriptor key, IMainSettings settings, IMethodsService method,
            IPropertyPool propertyPool, IEnumerable<DeviceTypeDescriptor> dictionaries, TestResultID result)
        {
            var data = new CheckConfigData();
            data.TargetDevice.Device.DeviceType = key;
            var avalableDeviceTypes = GetAllAvailableDeviceTypes(settings, dictionaries);
            var methods = method.MethodsForType(key);
            var res = new CheckConfigDevice(data, methods, avalableDeviceTypes, propertyPool, result);
            return res;
        }

        /// <summary>
        /// Получить список доступных типов устройств (объектов контроля)
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="dictionaries">Справочник функционала</param>
        /// <returns></returns>
        private static List<DeviceTypeDescriptor> GetAllAvailableDeviceTypes(IMainSettings settings,
            IEnumerable<DeviceTypeDescriptor> dictionaries)
        {
            var avalableDeviceTypes = new List<DeviceTypeDescriptor>();
            foreach (var deviceType in dictionaries)
            {
                if (!settings.Devices.Contains(deviceType))
                    continue;
                avalableDeviceTypes.Add(deviceType);
            }
            return avalableDeviceTypes;
        }
    }
}
