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
            const string sqlCreateRepairs = @"CREATE TABLE IF NOT EXISTS [Repairs](
                    [RepairId] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Timestamp] DATETIME NOT NULL);";
            // Описатели проверки
            const string sqlCreateRepairDescriptors = @"CREATE TABLE IF NOT EXISTS [RepairDescriptors](
                    [RepairId] integer NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [TargetDevice] integer,
                    [EthalonDevice] integer,
                    [Temperature] integer,
                    [TemperatureUnit] integer,
                    [Pressure] integer,
                    [PressureUnit] integer,
                    [Humidity] integer,
                    [HumidityUnit] integer);";
            // Описатели устройств
            const string sqlCreateDevices = @"CREATE TABLE IF NOT EXISTS [Devices](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Type] NTEXT,
                    [SerialNumber] NTEXT,
                    [DateLastVerification] integer);";
            // Описатели изммерительных каналов, сохраненные как Adjacency List
            const string sqlCreateChannels = @"CREATE TABLE IF NOT EXISTS [Channels](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            // Параметры проведеня проверерки, сохраненные как Adjacency List
            const string sqlCreateParameters = @"CREATE TABLE IF NOT EXISTS [Parameters](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            // Результаты проверерки, сохраненные как Adjacency List
            const string sqlCreateResults = @"CREATE TABLE IF NOT EXISTS [Results](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Id] DECIMAL NOT NULL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            /*const string sql =
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
                    [Data] NTEXT);" +
                // Словарь дополнительных данных проверок
                @"CREATE TABLE IF NOT EXISTS [Metadata](
                    [RepairId] DECIMAL NOT NULL REFERENCES Repairs([RepairId]),
                    [Key] NTEXT,
                    [Data] NTEXT);";
            context.Transaction(ts => ts.Connection.Execute(sql));*/
            context.Transaction(ts =>
            {
                ts.Connection.Execute(sqlCreateRepairs);
                ts.Connection.Execute(sqlCreateRepairDescriptors);
                ts.Connection.Execute(sqlCreateDevices);
                ts.Connection.Execute(sqlCreateChannels);
                ts.Connection.Execute(sqlCreateParameters);
                ts.Connection.Execute(sqlCreateResults);
            });
        }
    }
}
