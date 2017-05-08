using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    /// <summary>
    /// Загрузить список всех идентификаторов проверок
    /// </summary>
    public class LoadAllRapairs : IQuery<IEnumerable<Repair>>
    {
        public IEnumerable<Repair> Execute(IDbContext context)
        {
            const string sql = 
                @"SELECT * FROM [Repairs]";
            List<Repair> repairs = new List<Repair>();
            context.Transaction(ts => repairs.AddRange(ts.Connection.Query<Repair>(sql)));
            return repairs;
        }
    }
}
