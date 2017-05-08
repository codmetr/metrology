using System.Collections.Generic;
using System.Text;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    public class InsertOrUpdateParameter : ICommand
    {
        private readonly int _repairId;

        private readonly IEnumerable<Node> _nodes;
        
        public InsertOrUpdateParameter(Node node, int repairId)
            :this(new[] { node }, repairId) {}

        public InsertOrUpdateParameter(IEnumerable<Node> nodes, int repairId)
        {
            _nodes = nodes;
            _repairId = repairId;
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

        private string GetInsertOrUpdateSql(Node node)
        {
            const string sqlInsert = @"INSERT INTO [Parameters]"+
                                        " (RepairId, Id, ParrentId, Name, Val, TypeVal)" +
                                        " VALUES" +
                                        " ('{0}', '{1}', '{2}', {3}, '{4}', '5');";

            const string sqlUpdate = @"UPDATE [Parameters] Set" +
                                        " [ParrentId] = '{2}'," +
                                        " [Name] = '{3}'," +
                                        " [Val] = {4}," +
                                        " [TypeVal] = '{5}'" +
                                        " WHERE Id ='{1}' AND [RepairId] = '{0}';";
            var sql = node.IsNew ? sqlInsert : sqlUpdate;
            return string.Format(sql, _repairId, node.Id, node.ParentId, node.Name, node.Val==null?"NULL":string.Format("'{0}'",node.Val), node.TypeVal);
        }
    }
}
