﻿using Dapper;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class ClearTableChecks:ICommand
    {
        public void Execute(IDbContext context)
        {
            const string sql = @"DELETE FROM [Checks]";
            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
