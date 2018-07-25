using System.Drawing;
using ArchiveData.DTO;

namespace KipTM.Interfaces
{
    public class DeviceViewDescriptor
    {
        public DeviceTypeDescriptor Key { get; set; }

        public string Title { get; set; }

        public Bitmap SmallImg { get; set; }

        public Bitmap BigImg { get; set; }
    }
}
