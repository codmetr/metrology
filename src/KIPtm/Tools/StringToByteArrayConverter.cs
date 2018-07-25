using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class StringToByteArrayConverter
    {
        public static string ByteArrayToString(byte[] arr, int from, int to)
        {
            if (arr == null)
                return string.Empty;

            int len = arr.Length;
            if (from >= len)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = from; i < len && i < to; i++)
            {
                sb.Append(arr[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
