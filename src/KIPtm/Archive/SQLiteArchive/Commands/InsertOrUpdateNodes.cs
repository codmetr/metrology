using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    public class InsertOrUpdateNodes : ICommand
    {
        private readonly IEnumerable<Node> _nodes;
        private Action<string> _trace;
        private readonly string _table;

        public InsertOrUpdateNodes(Node node, string table, Action<string> trace = null)
        {
            _nodes = new [] { node};
            _table = table;
            _trace = trace;
        }


        public InsertOrUpdateNodes(IEnumerable<Node> nodes, string table, Action<string> trace = null)
        {
            _nodes = nodes;
            _table = table;
            _trace = trace;
        }


        public void Execute(IDbContext context)
        {
            if(!_nodes.Any())
                return;
            var sqlSb = new StringBuilder();
            foreach (var node in _nodes)
            {
                var sql = GetInsertOrUpdateSql(node, _table);
                sqlSb.Append(sql).Append("\n");
                node.IsNew = false;
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

        private static string GetInsertOrUpdateSql(Node node, string table)
        {
            string sqlInsert = string.Format(@"INSERT INTO [{0}]", table)+
                                        " (RepairId, Id, ParrentId, Name, Val, TypeVal)" +
                                        " VALUES" +
                                        " ('{0}', '{1}', '{2}', '{3}', {4}, '{5}');";

            string sqlUpdate = string.Format(@"UPDATE [{0}] Set", table) +
                                        " [ParrentId] = '{2}'," +
                                        " [Name] = '{3}'," +
                                        " [Val] = {4}," +
                                        " [TypeVal] = '{5}'" +
                                        " WHERE Id ='{1}' ;";
            var sql = node.IsNew ? sqlInsert : sqlUpdate;
            return string.Format(sql, node.RepairId, node.Id, node.ParrentId, node.Name, TreeParser.ValueToString(node), node.TypeVal);
        }
    }
}
