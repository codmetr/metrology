using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class ClearTableNodes:ICommand
    {
        private readonly string _table;

        public ClearTableNodes(string table)
        {
            _table = table;
        }

        public void Execute(IDbContext context)
        {
            string sql = string.Format(@"DELETE FROM [{0}]", _table);
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
