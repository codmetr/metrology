using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    /// <summary>
    /// Создать необходимый набор таблиц, если они не существуют
    /// </summary>
    public class CreateIfNotExistTable:ICommand
    {
        public void Execute(IDbContext context)
        {
                               
            const string sql =
                // Список идентификаторов проверок
                @"CREATE TABLE IF NOT EXISTS [Repairs](
                    [RepairId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Timestamp] DATETIME NOT NULL);" +
                // Результаты проверерки, сохраненные как Adjacency List
                @"CREATE TABLE IF NOT EXISTS [Results](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] DECIMAL NOT NULL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);" +
                // Параметры проведеня проверерки, сохраненные как Adjacency List
                @"CREATE TABLE IF NOT EXISTS [Parameters](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);" +
                // Словарь дополнительных данных проверок
                @"CREATE TABLE IF NOT EXISTS [Metadata](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Key] NTEXT,
                    [Data] NTEXT);";
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
