using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace KipTM.Model.Checks
{
    public interface ICheckMethodic
    {
        /// <summary>
        /// ������������� 
        /// </summary>
        /// <returns></returns>
        bool Init(IDictionary<string, object> parameters);

        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// ������ �����
        /// </summary>
        IEnumerable<ITestStep> Steps { get; } 

        /// <summary>
        /// ��������� ��������
        /// </summary>
        void Stop();
    }
}