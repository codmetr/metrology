using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Devices;

namespace KipTM.Model
{
    public class DictionaryService
    {
        /// <summary>
        /// Поддерживаетые типы устройств
        /// </summary>
        private readonly List<IDeviceTypeDescriptor> _deviceTypes;

        /// <summary>
        /// Поддерживаемые типы эталонов
        /// </summary>
        private readonly List<IDeviceTypeDescriptor> _ethalonTypes;


        public DictionaryService()
        {
            
        }

        /// <summary>
        /// Список типов поддерживаемых устройств
        /// </summary>
        public IEnumerable<IDeviceTypeDescriptor> DeviceTypes
        {
            get { return _deviceTypes; }
        }

        /// <summary>
        /// Список типов поддерживаемых эталонов
        /// </summary>
        public IEnumerable<IDeviceTypeDescriptor> EtalonTypes
        {
            get { return _ethalonTypes; }
        }

    }
}
