using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    public class RemoveNodes : ICommand
    {
        private readonly IEnumerable<Node> _nodes = null;
        private Action<string> _trace;
        private readonly string _table;

        public RemoveNodes(Node node, string table, Action<string> trace = null)
        {
            _nodes = new []{node};
            _table = table;
            _trace = trace;
        }

        public RemoveNodes(IEnumerable<Node> nodes, string table, Action<string> trace = null)
        {
            _nodes = nodes;
            _table = table;
            _trace = trace;
        }

        public void Execute(IDbContext context)
        {
            if (_nodes == null)
                return;
            if (!_nodes.Any())
                return;
            var sqlSb = new StringBuilder();
            string sqldelete = string.Format(@"DELETE FROM [{0}]", _table) + " WHERE Id = '{0}' AND RepairId = '{1}'";
            foreach (var node in _nodes)
            {
                if (node == null || node.IsNew)
                    continue;
                var sql = string.Format(sqldelete, node.Id, node.RepairId);
                sqlSb.Append(sql).Append("\n");
            }

            try
            {
                context.Transaction(ts => ts.Connection.Execute(sqlSb.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (_trace == null)
                    _trace(string.Format("SQL: {0}\nException:{1}", sqlSb.ToString(), ex.ToString()));
                throw;
            }
        }
    }
}
