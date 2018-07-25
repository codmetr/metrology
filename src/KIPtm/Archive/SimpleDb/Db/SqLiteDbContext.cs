using System;
using System.Data;
using System.Data.SQLite;
namespace SimpleDb.Db
{
    public class SqLiteDbContext : IDbContext
    {
        private readonly string _connectionString;

        public SqLiteDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public T Transaction<T>(Func<IDbTransaction, T> func)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = func(transaction);
                        transaction.Commit();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
           
        }

        public void Transaction(Action<IDbTransaction> func)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        func(transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
