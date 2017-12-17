using System;
using System.Linq;
using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    /// <summary>
    /// Создать новую запись в БД и получить её Id
    /// </summary>
    public class CreateNewRepair:IQuery<int>
    {
        private readonly DateTime _createTime;
        
        /// <summary>
        /// Создать новую запись в БД и получить её Id
        /// </summary>
        /// <param name="createTime">метка времени создания новой проверки</param>
        public CreateNewRepair(DateTime createTime)
        {
            _createTime = createTime;
        }
        
        public int Execute(IDbContext context)
        {
            const string sqlInsert = @"INSERT INTO [Repairs] (Timestamp) VALUES (@Timestamp);" +
                                     @"SELECT Id FROM [Repairs] WHERE rowid=last_insert_rowid();";
            return context.Transaction(ts => ts.Connection.Query<int>(sqlInsert, _createTime)).FirstOrDefault();
        }
    }
}
