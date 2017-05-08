using System.Collections.Generic;
using System.Linq;

namespace SQLiteArchive.Tree
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

        /// <summary>
        /// Получить дерево из линейного списка
        /// </summary>
        /// <param name="nodes">линейный список узлов</param>
        /// <returns>корень</returns>
        public static Node GetNodesFrom(IEnumerable<Node> nodes)
        {
            var nodeSet = nodes as List<Node> ?? nodes.ToList();
            IDictionary<long, Node> nodeDict = nodeSet.ToDictionary(n => n.Id);
            var rootId = nodeSet.Min(el => el.Id);
            var root = nodeDict[rootId];
            foreach (var node in nodeSet)
                if(nodeDict.ContainsKey(node.ParentId) && node.Id != root.Id)
                    nodeDict[node.ParentId].Childs.Add(node);

            return root;
        }

    }
}
