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
        private readonly Node _node;

        public InsertOrUpdate(Node node)
        {
            _node = node;
        }

        public void Execute(IDbContext context)
        {
            const string sqlInsert = @"INSERT INTO [Nodes]
                                        (Id, ParrentId, Name, Val)
                                        VALUES
                                        (@Id, @ParrentId, @Name, @Val)";

            const string sqlUpdate = @"UPDATE [Nodes] Set  
                                       [ParrentId] = @ParrentId,
                                       [Name] = @Name,
                                       [Val] = @Val
                                       WHERE Id =@Id";
            var sql = _node.IsNew ? sqlInsert : sqlUpdate;
            context.Transaction(ts => ts.Connection.Execute(sql, _node));
            _node.IsNew = true;
        }
    }
}
