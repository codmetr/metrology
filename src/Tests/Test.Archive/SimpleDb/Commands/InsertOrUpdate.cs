using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleDb.Db;

namespace SimpleDb.Commands
{
    public class InsertOrUpdate : ICommand
    {
        private int _id;
        private int _parrentId;
        private string _name;
        private string _val;

        public InsertOrUpdate(int id, int parrentId, string name, string val)
        {
            _id = id;
            _parrentId = parrentId;
            _name = name;
            _val = val;
        }

        public InsertOrUpdate(IEnumerable<InsertOrUpdate> items)
        {
            _id = id;
            _parrentId = parrentId;
            _name = name;
            _val = val;
        }

        public void Execute(IDbContext context)
        {
            const string sql = @"CREATE TABLE IF NOT EXISTS [Nodes] 
                        (
                            [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [ParrentId] integer NOT NULL,
                            [Name] char(255) NOT NULL UNIQUE,
                            [Val] char(255) NOT NULL UNIQUE
                        );";

            context.Transaction(ts => ts.Connection.Execute(sql));
        }
    }
}
