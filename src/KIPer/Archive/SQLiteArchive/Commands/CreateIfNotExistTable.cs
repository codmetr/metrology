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
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Timestamp] DATETIME NOT NULL);";
            // Описатели проверки
            const string sqlCreateRepairDescriptors = @"CREATE TABLE IF NOT EXISTS [RepairDescriptors](
                    [Id] integer NOT NULL REFERENCES Repairs([Id]),
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
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Type] NTEXT,
                    [SerialNumber] NTEXT,
                    [DateLastVerification] integer);";
            // Описатели изммерительных каналов, сохраненные как Adjacency List
            const string sqlCreateChannels = @"CREATE TABLE IF NOT EXISTS [Channels](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            // Параметры проведеня проверерки, сохраненные как Adjacency List
            const string sqlCreateParameters = @"CREATE TABLE IF NOT EXISTS [Config](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            // Результаты проверерки, сохраненные как Adjacency List
            const string sqlCreateResults = @"CREATE TABLE IF NOT EXISTS [Results](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] DECIMAL NOT NULL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);";
            /*const string sql =
                // Список идентификаторов проверок
                @"CREATE TABLE IF NOT EXISTS [Repairs](
                    [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                    [Timestamp] DATETIME NOT NULL);" +
                // Результаты проверерки, сохраненные как Adjacency List
                @"CREATE TABLE IF NOT EXISTS [Results](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] DECIMAL NOT NULL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);" +
                // Параметры проведеня проверерки, сохраненные как Adjacency List
                @"CREATE TABLE IF NOT EXISTS [Parameters](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Id] DECIMAL,
                    [ParentId] DECIMAL,
                    [Name] NTEXT,
                    [Val] NTEXT,
                    [TypeVal] DECIMAL);" +
                // Словарь дополнительных данных проверок
                @"CREATE TABLE IF NOT EXISTS [Metadata](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
                    [Key] NTEXT,
                    [Data] NTEXT);" +
                // Словарь дополнительных данных проверок
                @"CREATE TABLE IF NOT EXISTS [Metadata](
                    [Id] DECIMAL NOT NULL REFERENCES Repairs([Id]),
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
