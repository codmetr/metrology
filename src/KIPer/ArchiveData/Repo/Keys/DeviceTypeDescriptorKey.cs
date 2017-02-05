using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace ADTSData.Keys
{
    public static class DeviceTypeDescriptorKey
    {
        public static string GetKey(this DeviceTypeDescriptor obj)
        {
            return string.Format("{0}", obj.Model);
        }
    }
}
