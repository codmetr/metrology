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

        public List<CheckDto> Checks = new List<CheckDto>();
        private bool _isCreted = false;

        public void Load()
        {
            if (!File.Exists(_dbName))
                return;
            CreateTableIfNoExist();
            DoLoadNodes(ResultNodesTable, NodesRes);
            DoLoadNodes(ConfigNodesTable, NodesConf);

            var loadChecks = new LoadAllChecks();
            var checks = DataBase.Query(loadChecks);
            Checks.Clear();
            Checks.AddRange(checks);
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
            DataBase.Execute(new ClearTableNodes(ResultNodesTable));
            DataBase.Execute(new ClearTableNodes(ConfigNodesTable));
            DataBase.Execute(new ClearTableChecks());
        }

        public void AddResult(CheckDto check, IEnumerable<Node> nodes)
        {
            CreateTableIfNoExist();
            DataBase.Execute(new AddCheck(check, _trace));
            Checks.Add(check);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, ResultNodesTable, _trace));
            NodesRes.AddRange(nodes);
        }

        public void AddConfig(CheckDto check, IEnumerable<Node> nodes)
        {
            CreateTableIfNoExist();
            DataBase.Execute(new AddCheck(check, _trace));
            Checks.Add(check);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, ConfigNodesTable, _trace));
            NodesConf.AddRange(nodes);
        }

        public void UpdateRes(CheckDto check, IEnumerable<Node> newNodes, IEnumerable<Node> lessNodes, IEnumerable<Node> updateNodes)
        {
            CreateTableIfNoExist();
            Update(NodesRes, check, ResultNodesTable, newNodes, lessNodes, updateNodes);
        }

        public void UpdateConf(CheckDto check, IEnumerable<Node> newNodes, IEnumerable<Node> lessNodes, IEnumerable<Node> updateNodes)
        {
            CreateTableIfNoExist();
            Update(NodesConf, check, ConfigNodesTable, newNodes, lessNodes, updateNodes);
        }

        private void Update(List<Node> tNodes, CheckDto check, string table, IEnumerable<Node> newNodes, IEnumerable<Node> lessNodes, IEnumerable<Node> updateNodes)
        {
            DataBase.Execute(new UpdateCheck(check, _trace));

            var nodes = newNodes.ToList();
            nodes.AddRange(updateNodes);
            DataBase.Execute(new InsertOrUpdateNodes(nodes, table, _trace));
            tNodes.AddRange(newNodes);

            DataBase.Execute(new RemoveNodes(lessNodes, table, _trace));
            foreach (var lessNode in lessNodes)
            {
                tNodes.Remove(lessNode);
            }
        }

        public void Update(CheckDto check)
        {
            DataBase.Execute(new UpdateCheck(check, _trace));
        }

        public IEnumerable<Node> LoadRes(CheckDto check)
        {
            var load = new LoadNodes(ResultNodesTable, check.Id, _trace);
            DataBase.Execute(load);
            return load.Nodes;
        }

        public IEnumerable<Node> LoadConf(CheckDto check)
        {
            var load = new LoadNodes(ConfigNodesTable, check.Id, _trace);
            DataBase.Execute(load);
            return load.Nodes;
        }

        private IDbContext ConnectionContext
        {
            get { return _connectionContext = _connectionContext ?? new SqLiteDbContext(string.Format("Data Source={0};", _dbName)); }
        }

        private IDatabase DataBase
        {
            get { return _dataBase = _dataBase ?? new Database(ConnectionContext); }
        }

        private void CreateTableIfNoExist()
        {
            if(_isCreted)
                return;
            DataBase.Execute(new CreateTableNodes(ResultNodesTable));
            DataBase.Execute(new CreateTableNodes(ConfigNodesTable));
            DataBase.Execute(new CreateTables());
            _isCreted = true;
        }
    }
}
