using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    public class SqliteRepo : IRepository
    {
        private string path;

        private IEnumerable<DataRow> GetAllRows(out string error)
        {
            error = string.Empty;
            if (!File.Exists(path))
            {
                error = "file not found";
                return null;
            }

            using (var conn = new SQLiteConnection(path))
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT * from data");

                conn.Close();
            }
        }

        public bool Load(string path, out string error)
        {
            error = string.Empty;
            var data = GetAllRows(out error);

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
            if(File.Exists(path))
                File.Delete(path);
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;",path)))
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
