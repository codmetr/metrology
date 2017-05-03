using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class InsertOrUpdate : ICommand
    {
        private readonly IEnumerable<Node> _nodes;

        public InsertOrUpdate(Node node)
        {
            _nodes = new [] { node};
        }


        public InsertOrUpdate(IEnumerable<Node> nodes)
        {
            _nodes = nodes;
        }


        public void Execute(IDbContext context)
        {
            var sqlSb = new StringBuilder();
            foreach (var node in _nodes)
            {
                var sql = GetInsertOrUpdateSql(node);
                sqlSb.Append(sql).Append("\n");
                node.IsNew = true;
            }
            context.Transaction(ts => ts.Connection.Execute(sqlSb.ToString()));

        }

        private static string GetInsertOrUpdateSql(Node node)
        {
            const string sqlInsert = @"INSERT INTO [Nodes]"+
                                        " (Id, ParrentId, Name, Val, TypeVal)" +
                                        " VALUES" +
                                        " ('{0}', '{1}', '{2}', {3}, '{4}');";

            const string sqlUpdate = @"UPDATE [Nodes] Set" +
                                        " [ParrentId] = '{1}'," +
                                        " [Name] = '{2}'," +
                                        " [Val] = {3}," +
                                        " [TypeVal] = '{4}'" +
                                        " WHERE Id ='{0}' ;";
            var sql = node.IsNew ? sqlInsert : sqlUpdate;
            return string.Format(sql, node.Id, node.ParrentId, node.Name, node.Val==null?"NULL":string.Format("'{0}'",node.Val), node.TypeVal);
        }
    }
}
