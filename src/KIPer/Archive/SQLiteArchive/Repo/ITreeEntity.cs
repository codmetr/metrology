using System;
using System.Collections.Generic;

namespace SQLiteArchive.Repo
{
    public interface ITreeEntity
    {
        int Id { get; }
        string Key { get; set; }
        int ParrentId { get; }
        TreeEntity this[string key] { get; set; }
        string Value { get; set; }
        ValueWrapper Values { get; }
        IEnumerable<TreeEntity> Childs { get; }
    }
}
