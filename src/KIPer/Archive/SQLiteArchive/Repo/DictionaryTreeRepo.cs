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
            get
            {
                if (_childs.ContainsKey(key))
                    return _childs[key];
                _childs.Add(key, new TreeRepo());
                return _childs[key];
            }
            set
            {
                if (_childs.ContainsKey(key))
                    _childs[key] = value;
                else
                    _childs.Add(key, value);
            }
        }

    }
}
