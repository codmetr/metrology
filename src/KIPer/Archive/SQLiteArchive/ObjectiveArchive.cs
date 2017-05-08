using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLiteArchive.Commands;
using SQLiteArchive.Db;
using SQLiteArchive.Tree;

namespace SQLiteArchive
{
    public class ObjectiveArchive: IObjectiveArchive
    {
        private readonly string _dbName;

        public ObjectiveArchive(string dbName)
        {
            _dbName = dbName;
        }

        public void Create()
        {
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new CreateIfNotExistTable());
        }

        public int CreateNewRepair(DateTime timestamp)
        {
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new CreateIfNotExistTable());
            return db.Query(new CreateNewRepair(timestamp));
        }

        public IEnumerable<Repair> LoadAllRepairs()
        {
            if (!File.Exists(_dbName))
                return new Repair[0];
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            return db.Query(new LoadAllRapairs());
        }

        public void SaveResult<T>(int repairId, T result)
        {
            if (!File.Exists(_dbName))
                return;
            var treeNode = TreeParser.Convert(result, new Node(), new ItemDescriptor());
            var nodes = NodeLiner.GetNodesFrom(treeNode);
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new InsertOrUpdateResult(nodes, repairId));
        }

        public T LoadResult<T>(int repairId) where T : class
        {
            if(!File.Exists(_dbName))
                throw new FileNotFoundException(string.Format("DB file \"{0}\"", _dbName));
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            var nodesLine = db.Query(new LoadParameters(repairId));
            var root = NodeLiner.GetNodesFrom(nodesLine);
            object res;
            if (!TreeParser.TryParse(root, out res, typeof(T), new ItemDescriptor()))
                throw new InvalidCastException(string.Format("Can not parce to type \"{0}\" from Nodes", typeof(T)));
            return res as T;
        }

        public void SaveParameters<T>(int repairId, T parameters)
        {
            if (!File.Exists(_dbName))
                return;
            var treeNode = TreeParser.Convert(parameters, new Node(), new ItemDescriptor());
            var nodes = NodeLiner.GetNodesFrom(treeNode);
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new InsertOrUpdateParameter(nodes, repairId));
        }

        public T LoadParameters<T>(int repairId) where T : class
        {
            if (!File.Exists(_dbName))
                throw new FileNotFoundException(string.Format("DB file \"{0}\"", _dbName));
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            var nodesLine = db.Query(new LoadResults(repairId));
            var root = NodeLiner.GetNodesFrom(nodesLine);
            object res;
            if (!TreeParser.TryParse(root, out res, typeof (T), new ItemDescriptor()))
                throw new InvalidCastException(string.Format("Can not parce to type \"{0}\" from Nodes", typeof(T)));
            return res as T;
        }

        public void AddOrUpdateMetadata(int repairId, string key, string val)
        {
            if (!File.Exists(_dbName))
                return;
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            db.Execute(new InsertOrUpdateMetadata(key, val, repairId));
        }

        public IDictionary<string, string> GetAllMetadata(int repairId)
        {
            if (!File.Exists(_dbName))
                return new Dictionary<string, string>();
            var db = new Database(new SqLiteDbContext($"Data Source={_dbName};"));
            return db.Query(new LoadMetadatas(repairId)).ToDictionary(el=>el.Key, el=>el.Data);
        }
    }
}
