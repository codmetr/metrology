using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    public class DataRow
    {
        public readonly int Id;
        public readonly int ParrentId;
        public readonly string Value;

        public DataRow(int id, int parrentId, string value)
        {
            Id = id;
            ParrentId = parrentId;
            Value = value;
        }
    }
}
