using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace ADTSData.Keys
{
    public static class TestResultKey
    {
        public static string GetKey(this TestResult obj)
        {
            return string.Format("{0}_{1}_{2}", obj.CheckType, obj.Channel, obj.Timestamp.ToString("yyMMddHHmmssffff"));
        }
    }
}
