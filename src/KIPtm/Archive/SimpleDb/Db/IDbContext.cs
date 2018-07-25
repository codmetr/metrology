using System;
using System.Data;

namespace SimpleDb.Db
{
    public interface IDbContext
    {
        /// <summary>
        /// ��������� ����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T Transaction<T>(Func<IDbTransaction, T> func);

        /// <summary>
        /// ��������� ���������� 
        /// </summary>
        /// <param name="func"></param>
        void Transaction(Action<IDbTransaction> func);
    }
}