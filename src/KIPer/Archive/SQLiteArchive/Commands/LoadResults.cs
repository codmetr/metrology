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
    public class LoadResults : IQuery<IEnumerable<Node>>
    {
        private readonly int _repairId;
        private readonly List<Node> _nodes = new List<Node>();

        /// <summary>
        /// Загрузить результаты проверки
        /// </summary>
        /// <param name="repairId">Идентификатор проверки</param>
        public LoadResults(int repairId)
        {
            _repairId = repairId;
        }

        public IEnumerable<Node> Execute(IDbContext context)
        {
            const string sql = 
                @"SELECT
                    [Id],
                    [ParentId],
                    [Name],
                    [Val],
                    [TypeVal]
                FROM [Results] WHERE Id = @Id";
            _nodes.Clear();
            return context.Transaction(ts => ts.Connection.Query<Node>(sql, _repairId).Select(el =>
            {
                el.IsNew = false;
                if(el.Val!=null)
                    el.Val = TreeParser.ParceValue(el.Val.ToString(), el.TypeVal);
                else
                    Debug.WriteLine(el.Name);
                return el;
            }).ToList());
        }
    }
}
