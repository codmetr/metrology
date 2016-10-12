using System.Collections.Generic;
using CheckFrame.Archive;

namespace CheckFrame.Model.Checks
{
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