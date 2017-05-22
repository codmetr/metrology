using System.Collections.Generic;
using KipTM.Archive;
using KipTM.Model.Checks;

namespace KipTM.Interfaces.Checks
{
    /// <summary>
    /// �������� ��������
    /// </summary>
    public interface ICheckMethod
    {
        /// <summary>
        /// ������������� ��������
        /// </summary>
        string Key { get; }

        /// <summary>
        /// �������� ��������
        /// </summary>
        string Title { get; }

        /// <summary>
        /// �������� 
        /// </summary>
        /// <param name="propertyPool"></param>
        /// <returns></returns>
        object GetCustomConfig(IPropertyPool propertyPool);

        /// <summary>
        /// ������������� 
        /// </summary>
        /// <returns></returns>
        bool Init(object customConf);

        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// ������ �����
        /// </summary>
        IEnumerable<CheckStepConfig> Steps { get; } 

        /// <summary>
        /// ��������� ��������
        /// </summary>
        void Stop();
    }
}