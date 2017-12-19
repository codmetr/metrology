using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive.Commands
{
    public class LoadNodes : ICommand
    {
        private readonly long _repairId;
        private Action<string> _trace;
        private readonly List<Node> _nodes = new List<Node>();
        private readonly string _tableName;

        public LoadNodes(string tableName, long repairId = -1, Action<string> trace = null)
        {
            _tableName = tableName;
            _repairId = repairId;
            _trace = trace;
        }

        public IEnumerable<Node> Nodes { get { return _nodes; } }

        public void Execute(IDbContext context)
        {
            string sql = string.Format(@"Select * FROM [{0}]", _tableName);
            string sqlByRepairId = string.Format(@"Select * FROM [{0}] where RepairId = \'{1}\'", _tableName, _repairId);
            var resSql = _repairId >= 0 ? sqlByRepairId : sql;
            _nodes.Clear();
            if (_trace != null)
                _trace(string.Format("LoadNodes: call \"{0}\"", resSql));
            context.Transaction(ts =>
            {
                var queryRes = ts.Connection.Query<Node>(resSql).Select(el =>
                {
                    el.IsNew = false;
                    if (el.Val != null)
                        el.Val = TreeParser.ParceValue(el.Val.ToString(), el.TypeVal);
                    else
                        Debug.WriteLine(string.Format("In entity[Id = {0}, Name = {1}] value is null", el.Id, el.Name));
                    return el;
                }).ToList();
                _nodes.AddRange(queryRes);
            });
        }
    }
}
