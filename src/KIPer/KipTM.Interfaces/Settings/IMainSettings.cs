using System.Collections.Generic;

namespace KipTM.Settings
{
    public interface IMainSettings
    {
        List<DeviceTypeSettings> Devices { get; }
        List<DeviceSettings> LastEtalons { get; }
        List<DeviceSettings> LastDevices { get; }
        List<ComPortSettings> Ports { get; }
    }
}