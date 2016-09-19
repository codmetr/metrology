using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    interface IRepository
    {
        string GetByKey(string key);
        string SetByKey(string key, string value);
    }
}
