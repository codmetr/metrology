using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using SQLiteArchive.Repo;

namespace SQLiteArchive
{
    public class SqliteRepo : IRepository
    {
        private string path;

        private IEnumerable<DataRow> GetAllRows()
        {
            if (!File.Exists(path))
            {
                yield break;
            }

            using (var conn = new SQLiteConnection(path))
            {
                var cmd = new SQLiteCommand("SELECT * from data", conn);
                var reader = cmd.ExecuteReader();

                do
                {
                    var id = (int)reader["Id"];
                    var parrentId = (int)reader["parrent"];
                    var key = (string)reader["key"];
                    var val = (string)reader["value"];
                    yield return new DataRow(id, parrentId, key, val);
                } while (reader.NextResult());
                conn.Close();
            }
        }

        public bool Load()
        {
            IDictionary<int, TreeEntity> nodes = new Dictionary<int, TreeEntity>();
            var data = GetAllRows();
            foreach (var dataRow in data)
            {
                var item = new TreeEntity(dataRow.Id, dataRow.ParrentId) { Key = dataRow.Key, Value = dataRow.Value };
                if (!nodes.ContainsKey(dataRow.ParrentId))
                    nodes[dataRow.ParrentId][dataRow.Key] = item;

                nodes.Add(dataRow.Id, item);
            }
            return true;
        }

        public string GetByKey(string key)
        {
            throw new NotImplementedException();
        }

        public string SetByKey(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool CreateRepository(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", path)))
            {
                connection.Open();
                SQLiteCommand cmdCreateMainTable = new SQLiteCommand("CREATE TABLE Checks (" +
                                                          "idCheck INTEGER PRIMARY KEY," +
                                                          " CheckType TEXT," +
                                                          " Timestamp TEXT," +
                                                          " TargetDeviceKey TEXT," +
                                                          " IsEnable INTEGER," +
                                                          " IsVisable INTEGER);", connection);

                SQLiteCommand cmdCreateAttributesTable = new SQLiteCommand("CREATE TABLE Attributes (" +
                                                          " id INTEGER PRIMARY KEY," +
                                                          " idCheck INTEGER," +
                                                          " field TEXT," +
                                                          " value TEXT);", connection);
                cmdCreateMainTable.ExecuteNonQuery();
                cmdCreateAttributesTable.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }
    }
}
