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
    public class LoadAllChecks : IQuery<IEnumerable<CheckDto>>
    {
        public IEnumerable<CheckDto> Execute(IDbContext context)
        {
            const string sql =
                @"SELECT * FROM [Checks]";
            List<CheckDto> repairs = new List<CheckDto>();
            context.Transaction(ts => repairs.AddRange(ts.Connection.Query<CheckDto>(sql)));
            return repairs;
        }
    }
}
