using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace ADTSData.Keys
{
    public static class DeviceDescriptorKey
    {
        /// <summary>
        /// Стратегия получения ключа по описателю устройства
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetKey(this DeviceDescriptor obj)
        {
            return string.Format("{0}_{1}", obj.DeviceType.Model, obj.SerialNumber);
        }
    }
}
