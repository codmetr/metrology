using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    public class TreeEntity : SQLiteArchive.Repo.ITreeEntity
    {
        private readonly int _parrentId;
        private readonly int _id;
        private IDictionary<string, TreeEntity> _properties;
        private string _key;
        private string _value;

        public TreeEntity(int id, int parrentId)
        {
            _id = id;
            _parrentId = parrentId;
        }

        public TreeEntity(int parrentId)
            : this(MaxId.Next, parrentId)
        {}

        public int Id { get { return _id; } }

        public int ParrentId { get { return _parrentId; } }

        public string Key { get { return _key; } set { _key = value; }}

        public string Value { get { return _value; } set { _value = value; } }

        public TreeEntity this[string key]
        {
            get
            {
                if (_properties.ContainsKey(key))
                    return _properties[key];
                throw new IndexOutOfRangeException(string.Format("no object by key {0}", key));
            }
            set
            {
                if (!_properties.ContainsKey(key))
                    _properties.Add(key, value);
                else
                    _properties[key] = value;
            }
        }

        public IEnumerable<TreeEntity> Childs{get { return _properties.Values; }}
    }
}
