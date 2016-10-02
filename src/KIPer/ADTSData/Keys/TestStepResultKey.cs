using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace ADTSData.Keys
{
    public static class TestStepResultKey
    {
        public static string GetKey(this TestStepResult obj)
        {
            return string.Format("{0}_{1}_{2}", obj.CheckKey, obj.ChannelKey, obj.StepKey);
        }
    }
}
