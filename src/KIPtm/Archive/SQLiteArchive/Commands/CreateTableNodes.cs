using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class CreateTableNodes:ICommand
    {
        private readonly string _table;

        public CreateTableNodes(string table)
        {
            _table = table;
        }

        public void Execute(IDbContext context)
        {// AUTOINCREMENT
            string sql = string.Format(@"CREATE TABLE IF NOT EXISTS [{0}] 
                        (
                            [Id] integer PRIMARY KEY NOT NULL,
                            [RepairId] integer NOT NULL,
                            [ParrentId] integer NOT NULL,
                            [Name] char(255),
                            [Val] char(255),
                            [TypeVal] integer
                        );", _table);

            context.Transaction(ts =>
            {
                ts.Connection.Execute(sql);
            });
        }
    }
}
