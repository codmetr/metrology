using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    /// <summary>
    /// Загрузить результаты проверки
    /// </summary>
    public class LoadMetadatas : IQuery<IEnumerable<Metadata>>
    {
        private readonly int _repairId;
        
        /// <summary>
        /// Загрузить настройки проверки
        /// </summary>
        /// <param name="repairId">Идентификатор проверки</param>
        public LoadMetadatas(int repairId)
        {
            _repairId = repairId;
        }

        public IEnumerable<Metadata> Execute(IDbContext context)
        {
            const string sql =
                @"SELECT [Key], [Data]
                FROM [Results] WHERE RepairId = @RepairId";
            return context.Transaction(ts => ts.Connection.Query<Metadata>(sql, _repairId));
        }
    }
}
