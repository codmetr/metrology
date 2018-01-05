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
            const string sqlCreateRepairs = @"CREATE TABLE IF NOT EXISTS [Tests](
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, 
                    [CreateTime] DATETIME NOT NULL, 
                    [Timestamp] DATETIME NOT NULL, 
                    [TargetDeviceKey] NVARCHAR[512], 
                    [DeviceType] NVARCHAR[512], 
                    [SerialNumber] NVARCHAR[512], 
                    [CommonResult] NVARCHAR[512], 
                    [Note] NVARCHAR[512]);";
            context.Transaction(ts =>
            {
                ts.Connection.Execute(sqlCreateRepairs);
            });
        }
    }
}
