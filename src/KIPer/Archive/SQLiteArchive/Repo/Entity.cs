using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive.Repo
{
    public class Entity
    {
        private readonly int _id;
        
        public Entity(int id)
        {
            _id = id;
        }

        public Entity()
        {
            _id = MaxId.Next;
        }

        public int Id { get { return _id; } }

        public string AsString
        {
            get { return ""; }
        }
    }
}
