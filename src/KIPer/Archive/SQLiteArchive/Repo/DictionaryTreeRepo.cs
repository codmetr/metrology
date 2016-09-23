using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    public class DictionaryTreeRepo
    {
        private IDictionary<string, TreeRepo> _childs;

        public TreeRepo this[string key]
        {
            get { }
            set;
        }

    }
}
