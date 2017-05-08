using System.Collections.Generic;
using System.Text;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    public class InsertOrUpdateMetadata : ICommand
    {
        private readonly int _repairId;

        private readonly IDictionary<string, string> _parameters;
        
        public InsertOrUpdateMetadata(string key, string data, int repairId)
            :this(new Dictionary<string, string>(){ {key, data} }, repairId) {}

        public InsertOrUpdateMetadata(IDictionary<string, string> parameters, int repairId)
        {
            _parameters = parameters;
            _repairId = repairId;
        }


        public void Execute(IDbContext context)
        {
            var sqlSb = new StringBuilder();
            foreach (var parameter in _parameters)
            {
                var sql = GetInsertOrUpdateSql(parameter.Key, parameter.Value, _repairId);
                sqlSb.Append(sql).Append("\n");
            }
            context.Transaction(ts => ts.Connection.Execute(sqlSb.ToString()));

        }

        private string GetInsertOrUpdateSql(string key, string data, int repairId)
        {
            const string sql = @"INSERT OR REPLACE INTO [Metadata]" +
                                        " (RepairId, Key, Data)" +
                                        " VALUES" +
                                        " ('{0}', '{1}', '{2}');";
            return string.Format(sql, _repairId, key, data);
        }
    }
}
