using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class Remove : ICommand
    {
        private readonly Node _node = null;

        public Remove(Node node)
        {
            _node = node;
        }

        public void Execute(IDbContext context)
        {
            const string sql = @"DELETE [Nodes] WHERE Id = @Id";
            if(_node == null || _node.IsNew)
                return;
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
