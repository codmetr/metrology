using System.Collections.Generic;

namespace KipTM.Interfaces
{
    public interface ITamplateArchive<T>
    {
        /// <summary>
        /// ��������� ��� ������ ������������
        /// </summary>
        /// <returns></returns>
        Dictionary<string, T> GetArchive();

        /// <summary>
        /// ��������� ��������� ������������
        /// </summary>
        /// <returns></returns>
        T GetLast();
        /// <summary>
        /// ��������� ��������� ������������
        /// </summary>
        /// <param name="data"></param>
        void SetLast(T data);
        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conf"></param>
        void AddTemplate(string name, T conf);
        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conf"></param>
        void Update(string name, T conf);
        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="name"></param>
        void Remove(string name);
    }
}