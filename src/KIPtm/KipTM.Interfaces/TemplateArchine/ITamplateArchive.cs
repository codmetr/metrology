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
        void AddTemplate(string name, T conf);
        void Update(string name, T conf);
        void Remove(string name);
    }
}