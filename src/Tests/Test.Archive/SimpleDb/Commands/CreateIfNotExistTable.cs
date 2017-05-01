using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class CreateIfNotExistTable:ICommand
    {
        public void Execute(IDbContext context)
        {
            const string sql = @"CREATE TABLE IF NOT EXISTS [Nodes] 
                        (
                            [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [ParrentId] integer NOT NULL,
                            [Name] char(255),
                            [Val] char(255)
                        );";

            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
