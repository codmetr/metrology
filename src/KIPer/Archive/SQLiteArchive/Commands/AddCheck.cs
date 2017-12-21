using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class AddCheck : ICommand
    {
        private CheckDto _check;
        private Action<string> _trace;

        public AddCheck(CheckDto check, Action<string> trace = null)
        {
            _check = check;
            _trace = trace;
        }

        public void Execute(IDbContext context)
        {
            context.Transaction(ts =>
                {
                    long id = ts.Connection.Insert(_check, ts);
                    _check.Id = id;
                    ts.Connection.Update(_check, ts);
                }
            );
        }
    }
}
