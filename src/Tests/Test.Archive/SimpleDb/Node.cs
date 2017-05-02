using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimpleDb.Db;

namespace SimpleDb
{
    [DebuggerDisplay("{Name}:{Val}")]
    public class Node:Entity
    {
        private static int _maxId = 0;
        private List<Node> _childs = new List<Node>();
        private Node _parrent;

        public static int GetNewId()
        {
            return _maxId++;
        }

        public Node()
        {
            Id = GetNewId();
        }


        public Node(int id)
        {
            Id = id;
        }

        public string Name { get; set; }

        //public int Id { get; set; }

        public Node Parrent
        {
            get { return _parrent; }
            set
            {
                _parrent = value;
                ParrentId = _parrent.Id;
            }
        }

        public long ParrentId { get; set; }

        public List<Node> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public object Val { get; set; }

        public int TypeVal { get; set; }

        public Node this[string key]
        {
            get
            {
                var res = Childs.FirstOrDefault(item => item.Name == key);
                return res;
            }
            set
            {
                var node = Childs.FirstOrDefault(item => item.Name == key);
                if (value == node)
                    return;
                if (node != null)
                    Childs.Remove(node);
                Childs.Add(value);
            }
        }
    }
}