using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Конфигурация 
    /// </summary>
    public class CheckConfigData
    {
        public string CheckTypeKey;
        public string TargetTypeKey;
        public DeviceTypeDescriptor TargetType;
        public string EthalonTypeKey;
        public DeviceTypeDescriptor EthalonType;
        public DeviceDescriptor Ethalon;
        public bool IsAnalogEthalon;
    }
}
