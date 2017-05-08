using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SQLiteArchive.Db;

namespace SQLiteArchive.Tree
{
    [DebuggerDisplay("ID:{Id} PId:{ParentId} {Name}:{Val}")]
    public class Node:Entity
    {
        private static int _maxId = 0;
        private List<Node> _childs = new List<Node>();
        private Node _parent;

        /// <summary>
        /// Метод получени нового GUID
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Имя узла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        public Node Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                ParentId = _parent.Id;
            }
        }

        /// <summary>
        /// Идентификатор родителя 
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// Дети
        /// </summary>
        public List<Node> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        /// <summary>
        /// Значение
        /// </summary>
        public object Val { get; set; }

        /// <summary>
        /// Тип значения 
        /// </summary>
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