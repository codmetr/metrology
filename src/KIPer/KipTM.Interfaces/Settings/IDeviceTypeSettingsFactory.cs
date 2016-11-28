using System.Collections.Generic;

namespace KipTM.Settings
{
    public interface IDeviceTypeSettingsFactory
    {
        IEnumerable<DeviceTypeSettings> GetDefault();
    }
}