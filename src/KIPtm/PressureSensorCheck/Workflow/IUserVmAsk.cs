using System;
using System.Threading;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// ���������� ������ ��� ���������� �������� ������������
    /// </summary>
    public interface IUserVmAsk
    {
        /// <summary>
        /// ���������
        /// </summary>
        string Note { get; set; }

        /// <summary>
        /// ����������� ������ � ��������������
        /// </summary>
        bool IsAsk { get; set; }

        /// <summary>
        /// ���������� �������� �� ������������� 
        /// </summary>
        /// <param name="accept"></param>
        void SetAcceptAction(Action accept);

        /// <summary>
        /// �������� �������� �� �������������
        /// </summary>
        void ResetSetAcceptAction();

        /// <summary>
        /// ����� ���������� ����
        /// </summary>
        /// <param name="title">���������</param>
        /// <param name="msg">���������</param>
        /// <param name="cancel">������</param>
        void AskModal(string title, string msg, CancellationToken cancel);
    }
}