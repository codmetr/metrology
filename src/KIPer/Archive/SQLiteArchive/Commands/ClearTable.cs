using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class ClearTable:ICommand
    {
        public void Execute(IDbContext context)
        {
            const string sql = @"DELETE FROM [Nodes]";
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
