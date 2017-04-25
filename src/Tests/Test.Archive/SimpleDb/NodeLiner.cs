using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDb
{
    public static class NodeLiner
    {
        /// <summary>
        /// Получить весь список узлов и листьев дерева (включая корень)
        /// </summary>
        /// <param name="root">корень</param>
        /// <returns></returns>
        public static List<Node> GetNodesFrom(Node root)
        {
            var nodes = new List<Node>() { root };
            foreach (var child in root.Childs)
            {
                if (child == null)
                    continue;
                var allCildNodes = GetNodesFrom(child);
                nodes.AddRange(allCildNodes);
            }
            return nodes;
        }
    }
}
