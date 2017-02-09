using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Конфигурация конкретной проверки
    /// </summary>
    public class CheckConfigData
    {
        /// <summary>
        /// Ключ типа методики проверки
        /// </summary>
        public string CheckTypeKey;
        /// <summary>
        /// Ключ типа проверяемого устройства
        /// </summary>
        public string TargetTypeKey;
        /// <summary>
        /// Тип проверяемого устройства
        /// </summary>
        public DeviceTypeDescriptor TargetType;
        /// <summary>
        /// Ключ типа эталона
        /// </summary>
        public string EthalonTypeKey;
        /// <summary>
        /// Тип эталона
        /// </summary>
        public DeviceTypeDescriptor EthalonType;
        /// <summary>
        /// Описатель эталона
        /// </summary>
        public DeviceDescriptor Ethalon;
        /// <summary>
        /// Эталон - устройство без аппаратного интерфейса
        /// </summary>
        public bool IsAnalogEthalon;
    }
}
