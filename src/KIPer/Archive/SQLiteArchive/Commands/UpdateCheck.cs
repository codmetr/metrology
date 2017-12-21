using System;
using System.Data;
using Dapper.Contrib.Extensions;
using SQLiteArchive.Db;

namespace SQLiteArchive.Commands
{
    public class UpdateCheck : ICommand
    {
        private CheckDto _check;
        private Action<string> _trace;

        public UpdateCheck(CheckDto check, Action<string> trace = null)
        {
            _check = check;
            _trace = trace;
        }

        public void Execute(IDbContext context)
        {
            context.Transaction(DoUpdate);
        }

        private void DoUpdate(IDbTransaction ts)
        {
            ts.Connection.Update(_check, ts);
        }
    }
}
