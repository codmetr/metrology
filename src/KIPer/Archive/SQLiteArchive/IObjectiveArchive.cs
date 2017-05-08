using System;
using System.Collections.Generic;

namespace SQLiteArchive
{
    /// <summary>
    /// ��������� ������
    /// </summary>
    public interface IObjectiveArchive
    {
        /// <summary>
        /// ������� ����� ������ � �������� � ���������� � ID
        /// </summary>
        /// <returns></returns>
        int CreateNewRepair(DateTime timestamp);

        /// <summary>
        /// �������� ID ���� ��������
        /// </summary>
        /// <returns>������ ID ���� ��������</returns>
        IEnumerable<Repair> LoadAllRepairs();

        /// <summary>
        /// ��������� ��������� �� ��������
        /// </summary>
        /// <typeparam name="T">��� ����������</typeparam>
        /// <param name="repairId">ID ��������</param>
        /// <param name="result">���������</param>
        void SaveResult<T>(int repairId, T result);

        /// <summary>
        /// ��������� ��������� ��������
        /// </summary>
        /// <typeparam name="T">��� ����������</typeparam>
        /// <param name="repairId">ID ��������</param>
        T LoadResult<T>(int repairId) where T : class;

        /// <summary>
        /// ��������� ��������� �� ��������
        /// </summary>
        /// <typeparam name="T">��� ����������</typeparam>
        /// <param name="repairId">ID ��������</param>
        /// <param name="parameters">���������</param>
        void SaveParameters<T>(int repairId, T parameters);

        /// <summary>
        /// ��������� ��������� ��������
        /// </summary>
        /// <typeparam name="T">��� ����������</typeparam>
        /// <param name="repairId">ID ��������</param>
        T LoadParameters<T>(int repairId) where T : class;

        /// <summary>
        /// �������� ��� �������� ���������� �� ��������
        /// </summary>
        /// <param name="repairId">ID ��������</param>
        /// <param name="key">����� ����������</param>
        /// <param name="val">����������</param>
        void AddOrUpdateMetadata(int repairId, string key, string val);

        /// <summary>
        /// �������� ��� ���������� �� ��������
        /// </summary>
        /// <param name="repairId">ID ��������</param>
        IDictionary<string, string> GetAllMetadata(int repairId);

    }
}