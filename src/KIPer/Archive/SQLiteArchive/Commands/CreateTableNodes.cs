using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class CreateTableNodes:ICommand
    {
        public void Execute(IDbContext context)
        {// AUTOINCREMENT
            const string sql = @"CREATE TABLE IF NOT EXISTS [Nodes] 
                        (
                            [Id] integer PRIMARY KEY NOT NULL,
                            [RepairId] integer NOT NULL,
                            [ParrentId] integer NOT NULL,
                            [Name] char(255),
                            [Val] char(255),
                            [TypeVal] integer
                        );";

            context.Transaction(ts =>
            {
                ts.Connection.Execute(sql);
            });
        }
    }
}
