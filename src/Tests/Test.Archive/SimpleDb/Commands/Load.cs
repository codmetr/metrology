using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class Load : ICommand
    {
        private readonly List<Node> _nodes = new List<Node>();

        public IEnumerable<Node> Nodes { get { return _nodes; } }

        public void Execute(IDbContext context)
        {
            const string sql = @"Select * FROM [Nodes]";
            _nodes.Clear();
            context.Transaction(ts => _nodes.AddRange(ts.Connection.Query<Node>(sql).Select(el =>
            {
                el.IsNew = false;
                return el;
            }).ToList()));
        }
    }
}
