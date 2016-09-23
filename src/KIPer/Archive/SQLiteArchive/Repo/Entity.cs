using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    public class Entity
    {
        private readonly int _Id;
        
        public Entity(int id)
        {
            _Id = id;
        }

        public Entity()
        {
            _Id = MaxId.Next;
        }

        public int Id { get { return _id; } }

        public string AsString
        {
            get { return ""; }
        }
    }
}
