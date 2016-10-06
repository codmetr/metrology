using System;
using System.Collections.Generic;

namespace SQLiteArchive.Repo
{
    public interface ITreeEntity
    {
        int Id { get; }
        string Key { get; set; }
        int ParrentId { get; }
        ITreeEntity this[string key] { get; set; }
        string Value { get; set; }
        ValueWrapper Values { get; }
        IEnumerable<ITreeEntity> Childs { get; }
        ITreeEntity AddRange(IEnumerable<ITreeEntity> items);
        ITreeEntity RemoveRange(IEnumerable<ITreeEntity> items);
        ITreeEntity SetKey(string key);
        ITreeEntity SetParrent(ITreeEntity parrent);
    }
}
