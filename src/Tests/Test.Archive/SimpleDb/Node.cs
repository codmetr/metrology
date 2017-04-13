using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleDb
{
    [DebuggerDisplay("{Name}:{Value}")]
    public class Node
    {
        private static int _maxId = 0;
        private List<Node> _childs = new List<Node>();

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

        public int Id { get; set; }

        public Node Parrent { get; set; }

        public List<Node> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public object Value { get; set; }

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