using System;
using System.Data;

namespace SimpleDb.Db
{
    public interface IDbContext
    {
        /// <summary>
        /// Выполнить транзакцию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T Transaction<T>(Func<IDbTransaction, T> func);

        /// <summary>
        /// Выполнить транзакцию 
        /// </summary>
        /// <param name="func"></param>
        void Transaction(Action<IDbTransaction> func);
    }
}