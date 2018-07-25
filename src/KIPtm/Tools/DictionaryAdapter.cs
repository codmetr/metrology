using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class DictionaryAdapter<Tres, Tindex>
    {
        private readonly Func<Tindex, Tres> _indexator;

        public DictionaryAdapter(Func<Tindex, Tres> indexator)
        {
            _indexator = indexator;
        }

        public Tres this[Tindex index]
        {
            get { return _indexator(index); }
        }
    }
}
