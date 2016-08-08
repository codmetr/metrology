using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public class ArchiveBase
    {

        private List<ArchivedKeyValuePair> _data;

        public static ArchiveBase LoadFromFile(string path, ArchiveBase defArchive)
        {
            // TODO Реализовать!!!
            return defArchive;
        }
        
        public ArchiveBase()
        {
            Data = new List<ArchivedKeyValuePair>();
        }

        public ArchiveBase(List<ArchivedKeyValuePair> data)
            : this()
        {
            Data = data;
        }


        /// <summary>
        /// Данные
        /// </summary>
        public List<ArchivedKeyValuePair> Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public ArchiveBase GetArchive(string key)
        {
            var first = _data.First(el => el.Key == key);
            if (first == null)
                throw new KeyNotFoundException(string.Format("Not found archive by key [{0}]", key));
            if (!(first.Value is List<ArchivedKeyValuePair>))
                throw new InvalidCastException(
                    string.Format("Can not cast element by key[{0}] with type [{1}] to target type [{2}]", key,
                        first.Value.GetType(), typeof (List<ArchivedKeyValuePair>)));
            return new ArchiveBase(first.Value as List<ArchivedKeyValuePair>);
        }

        public ArchiveBase CreateArchive(string key)
        {
            if(_data.Any(el => el.Key == key))
                throw new DuplicateNameException(string.Format("Try add value for exicted key [{0}]", key));
            var res = new List<ArchivedKeyValuePair>();
            _data.Add(new ArchivedKeyValuePair(key, res));
            return new ArchiveBase(res);
        }
    }
}