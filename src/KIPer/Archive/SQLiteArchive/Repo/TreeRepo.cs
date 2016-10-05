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
        private int _parrentId;
        private readonly int _id;
        private IDictionary<string, TreeEntity> _properties = new Dictionary<string, TreeEntity>();
        private string _key;
        private string _value;
        private ValueWrapper _values;

        public TreeEntity(int id, int parrentId)
        {
            _id = id;
            _parrentId = parrentId;
            _values = new ValueWrapper(this);
        }

        public TreeEntity(int parrentId)
            : this(MaxId.Next, parrentId)
        {}

        public TreeEntity()
            : this(MaxId.Next, 0)
        {}

        public static TreeEntity Make(int parrentId, string value)
        {
            var res = new TreeEntity(parrentId);
            res.Value = value;
            return res;
        }

        public int Id { get { return _id; } }

        public int ParrentId { get { return _parrentId; } }

        public string Key { get { return _key; } set { _key = value; }}

        public string Value { get { return _value; } set { _value = value; } }

        public ValueWrapper Values
        {
            get { return _values; }
        }

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
                value.Key = key;
                value._parrentId = Id;
            }
        }

        public IEnumerable<TreeEntity> Childs
        {
            get { return _properties.Values; }
        }

        public TreeEntity AddRange(IEnumerable<TreeEntity> items)
        {
            foreach (var item in items)
            {
                this[item.Key] = item;
            }
            return this;
        }

        public TreeEntity RemoveRange(IEnumerable<TreeEntity> items)
        {
            foreach (var item in items)
            {
                if (_properties.ContainsKey(item.Key))
                    _properties.Remove(item.Key);
            }
            return this;
        }

        public TreeEntity SetKey(string key)
        {
            _key = key;
            return this;
        }
    }
}
