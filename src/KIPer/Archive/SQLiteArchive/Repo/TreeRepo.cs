using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    public class TreeRepo
    {
        private readonly int _Id;
        private IDictionary<string, Entity> _properties;
        private DictionaryTreeRepo _childs;

        public TreeRepo(int id)
        {
            _Id = id;
        }

        public TreeRepo()
        {
            _Id = MaxId.Next;
        }

        public int Id { get { return _id; } }

        public Entity this[string key]
        {
            get
            {
                if (!_properties.ContainsKey(key))
                    _properties.Add(key, string.Empty);
                return _properties[key];
            }
            set
            {
                if (!_properties.ContainsKey(key))
                    _properties.Add(key, value);
                else
                    _properties[key] = value;
            }
        }

        public DictionaryTreeRepo Childs { get { return _childs; } }
    }
}
