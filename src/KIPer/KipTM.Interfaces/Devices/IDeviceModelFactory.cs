using KipTM.Model;
using MainLoop;

namespace KipTM.Interfaces.Checks
{

    public interface IDeviceModelFactory
    {
        object GetModel(ILoops loops, IDeviceManager deviceManager);
    }
}
