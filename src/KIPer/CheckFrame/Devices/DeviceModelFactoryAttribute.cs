using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Маркер фабрики модели устройства
    /// </summary>
    public class DeviceModelFactoryAttribute:Attribute
    {
        public DeviceModelFactoryAttribute(Type modelType)
        {
            ModelType = modelType;
        }

        public Type ModelType { get; private set; }
    }
}
