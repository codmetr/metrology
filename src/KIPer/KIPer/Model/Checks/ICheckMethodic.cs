using System;

namespace KipTM.Model.Checks
{
    public interface ICheckMethodic
    {
        /// <summary>
        /// ������������� 
        /// </summary>
        /// <returns></returns>
        bool Init();

        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="GetRealValue"></param>
        /// <param name="GetAccept"></param>
        /// <returns></returns>
        bool Start(string channels, Func<double> GetRealValue, Func<bool> GetAccept );

        /// <summary>
        /// ������
        /// </summary>
        void Cancel();
    }
}