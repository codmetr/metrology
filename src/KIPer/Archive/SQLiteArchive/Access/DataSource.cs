using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLiteArchive.Commands;
using SQLiteArchive.Db;
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

        private const string ResultNodesTable = "ResNodes";
        private const string ConfigNodesTable = "ConfNodes";

        private readonly string _dbName;
        public DataSource(string dbName = "test.db", Action<string> trace = null)
        {
            _dbName = dbName;
            _trace = trace;
        }

        public List<Node> NodesRes = new List<Node>();
        public List<Node> NodesConf = new List<Node>();

        public List<CheckDto> Repairs = new List<CheckDto>();
        private bool _isCreted = false;

        public void Load()
        {
            if (!File.Exists(_dbName))
                return;
            CreateTableIfNoExist();
            DoLoadNodes(ResultNodesTable, NodesRes);
            DoLoadNodes(ConfigNodesTable, NodesConf);

            var loadRepairs = new LoadAllRepairs();
            var repairs = DataBase.Query(loadRepairs);
            Repairs.Clear();
            Repairs.AddRange(repairs);
        }

        private void DoLoadNodes(string table, List<Node> nodes)
        {
            var loadNodes = new LoadNodes(table, trace: _trace);
            DataBase.Execute(loadNodes);
            nodes.Clear();
            nodes.AddRange(loadNodes.Nodes);
            if (nodes.Any())
            {
                Node.SetMaxId((int)nodes.Max(el => el.Id) + 1);
            }
        }

        public void Save()
        {
            CreateTableIfNoExist();
            DataBase.Execute(new InsertOrUpdateNodes(NodesRes, ResultNodesTable, _trace));
            DataBase.Execute(new InsertOrUpdateNodes(NodesConf, ConfigNodesTable, _trace));
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
