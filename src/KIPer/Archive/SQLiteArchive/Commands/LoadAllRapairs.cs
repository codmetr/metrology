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
    public class LoadAllRapairs : IQuery<IEnumerable<RepairDto>>
    {
        public IEnumerable<RepairDto> Execute(IDbContext context)
        {
            const string sql = 
                @"SELECT * FROM [Repairs]";
            List<RepairDto> repairs = new List<RepairDto>();
            context.Transaction(ts => repairs.AddRange(ts.Connection.Query<RepairDto>(sql)));
            return repairs;
        }
    }
}
