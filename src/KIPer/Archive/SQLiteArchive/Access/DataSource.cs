using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLiteArchive.Commands;
using SQLiteArchive.Db;
using SQLiteArchive.Db.DTO;
using SQLiteArchive.Tree;

namespace SQLiteArchive
{
    /// <summary>
    /// Механизм сохранения и загрузки
    /// </summary>
    public class DataSource
    {
        private IDbContext _connectionContext = null;
        private IDatabase _dataBase = null;
        private Action<string> _trace;

        private readonly string _dbName;
        private IList<Node> _nodes = new List<Node>(); 

        public DataSource(string dbName = "test.db", Action<string> trace = null)
        {
            _dbName = dbName;
            _trace = trace;
        }

        public List<Node> Nodes = new List<Node>();

        public List<CheckDto> Repairs = new List<CheckDto>();
        private bool _isCreted = false;

        public void Load()
        {
            if (!File.Exists(_dbName))
                return;
            CreateTableIfNoExist();
            var loadNodes = new LoadNodes(trace:_trace);
            DataBase.Execute(loadNodes);
            Nodes.Clear();
            Nodes.AddRange(loadNodes.Nodes);
            if (Nodes.Any())
            {
                Node.SetMaxId((int)Nodes.Max(el => el.Id)+1);
            }

            var loadRepairs = new LoadAllRepairs(_trace);
            DataBase.Execute(loadRepairs);
            Repairs.Clear();
            Repairs.AddRange(loadRepairs.Repairs);
        }

        public void Save()
        {
            CreateTableIfNoExist();
            DataBase.Execute(new InsertOrUpdateNodes(Nodes, _trace));
        }

        public void Clear()
        {
            if (!File.Exists(_dbName))
                return;
            DataBase.Execute(new ClearTableNodes());
            DataBase.Execute(new ClearTableRepairs());
        }

        public void Add(CheckDto check, IEnumerable<Node> nodes)
        {
            CreateTableIfNoExist();
            DataBase.Execute(new AddRepair(check, _trace));
            Repairs.Add(check);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, _trace));
            Nodes.AddRange(nodes);
        }

        public void Update(CheckDto check, IEnumerable<Node> newNodes, IEnumerable<Node> lessNodes, IEnumerable<Node> updateNodes)
        {
            CreateTableIfNoExist();
            DataBase.Execute(new UpdateRepair(check, _trace));

            var nodes = newNodes.ToList();
            nodes.AddRange(updateNodes);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, _trace));
            Nodes.AddRange(newNodes);

            DataBase.Execute(new RemoveNodes(lessNodes, _trace));
            foreach (var lessNode in lessNodes)
            {
                Nodes.Remove(lessNode);
            }
        }

        public void Update(CheckDto check)
        {
            DataBase.Execute(new UpdateRepair(check, _trace));
        }

        public void Update(IEnumerable<Node> newNodes, IEnumerable<Node> lessNodes, IEnumerable<Node> updateNodes)
        {
            CreateTableIfNoExist();
            var nodes = newNodes.ToList();
            nodes.AddRange(updateNodes);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, _trace));
            Nodes.AddRange(newNodes);

            DataBase.Execute(new RemoveNodes(lessNodes, _trace));
            foreach (var lessNode in lessNodes)
            {
                Nodes.Remove(lessNode);
            }
        }

        public IEnumerable<Node> Load(CheckDto check)
        {
            var load = new LoadNodes(check.Id, _trace);
            DataBase.Execute(load);
            return load.Nodes;
        }

        private IDbContext ConnectionContext
        {
            get { return _connectionContext = _connectionContext ?? new SqLiteDbContext(string.Format("Data Source={0};", _dbName), _trace); }
        }

        private IDatabase DataBase
        {
            get { return _dataBase = _dataBase ?? new Database(ConnectionContext); }
        }

        private void CreateTableIfNoExist()
        {
            if(_isCreted)
                return;
            DataBase.Execute(new CreateTableNodes());
            DataBase.Execute(new CreateTableRepairs());
            _isCreted = true;
        }
    }
}
