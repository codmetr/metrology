using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using KipTM.Archive;

namespace KipTM.Model.Checks
{
    public interface ICheckMethod
    {
        /// <summary>
        /// �������� ��������
        /// </summary>
        string Title { get; }

        /// <summary>
        /// ������������� 
        /// </summary>
        /// <returns></returns>
        bool Init(IPropertyPool propertyes);

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