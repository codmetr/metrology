using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    /// <summary>
    /// Создать необходимый набор таблиц, если они не существуют
    /// </summary>
    public class CreateTables:ICommand
    {
        public void Execute(IDbContext context)
        {
            // Список идентификаторов проверок
            const string sqlCreateRepairs = @"CREATE TABLE IF NOT EXISTS [Checks](
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Timestamp] DATETIME NOT NULL);";
            context.Transaction(ts =>
            {
                ts.Connection.Execute(sqlCreateRepairs);
            });
        }
    }
}
