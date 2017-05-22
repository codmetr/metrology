using System.Drawing;

namespace KipTM.Interfaces
{
    public class DeviceViewDescriptor
    {
        public string Key { get; set; }

        public string Title { get; set; }

        public Bitmap SmallImg { get; set; }

        public Bitmap BigImg { get; set; }
    }
}
