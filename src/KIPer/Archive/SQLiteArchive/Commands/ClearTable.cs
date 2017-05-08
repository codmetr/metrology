using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class ClearTable:ICommand
    {
        public void Execute(IDbContext context)
        {
            const string sql = @"DELETE FROM [Nodes]";
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
