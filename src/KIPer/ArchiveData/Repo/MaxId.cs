using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    internal class MaxId
    {
        private static int _maxIn = 0;

        public static int Next { get { return _maxIn++; } }
    }
}
