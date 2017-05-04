using System;
using System.Collections.Generic;
using System.IO;
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

        public List<Node> Nodes = new List<Node>();

        public void Load()
        {
            if(!File.Exists(_dbName))
                return;
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            var load = new Load();
            db.Execute(load);
            Nodes.Clear();
            Nodes.AddRange(load.Nodes);
        }


        public void Save()
        {
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new CreateIfNotExistTable());
            db.Execute(new InsertOrUpdate(Nodes));
        }

        public void Clear()
        {
            if (!File.Exists(_dbName))
                return;
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new ClearTable());
        }
    }
}
