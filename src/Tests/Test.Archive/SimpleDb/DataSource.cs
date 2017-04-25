using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDb.Commands;
using SimpleDb.Db;

namespace SimpleDb
{
    public class DataSource
    {
        private readonly string _dbName;
        private IList<Node> _nodes = new List<Node>(); 

        public DataSource(string dbName = "test.db")
        {
            _dbName = dbName;
        }

        public List<Node> Nodes;

        public void Save()
        {
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new CreateNodesTable());
            db.Execute(new CreateNodesTable());
        }
    }
}
