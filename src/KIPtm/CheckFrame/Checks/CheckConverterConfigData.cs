using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Конфигурация конкретной проверки конвертера
    /// </summary>
    public class CheckConverterConfigData
    {
        /// <summary>
        /// Ключ типа методики проверки
        /// </summary>
        public string CheckTypeKey;
        /// <summary>
        /// Тип проверяемого устройства
        /// </summary>
        public DeviceTypeDescriptor TargetType;
        /// <summary>
        /// Ключ типа эталона
        /// </summary>
        public string EtalonTypeKey;
        /// <summary>
        /// Тип эталона
        /// </summary>
        public DeviceTypeDescriptor EtalonType;
        /// <summary>
        /// Описатель эталона
        /// </summary>
        public DeviceDescriptor Etalon;
        /// <summary>
        /// Эталон - устройство без аппаратного интерфейса
        /// </summary>
        public bool IsAnalogEtalon;
    }
}
