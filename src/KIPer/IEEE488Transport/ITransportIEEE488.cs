namespace IEEE488
{
    public interface ITransportIEEE488
    {
        /// <summary>
        /// ������� �����������
        /// </summary>
        /// <param name="address">����� �������</param>
        /// <returns>true - ������� ������������</returns>
        bool Open(int address);
        
        /// <summary>
        /// ������� �����������
        /// </summary>
        /// <param name="address"></param>
        /// <returns>true - ������� ����������� ��� ������</returns>
        bool Close(int address);
        
        /// <summary>
        /// ������� �������
        /// </summary>
        /// <param name="data">�������</param>
        /// <returns>������� ������� �������</returns>
        bool Send(string data);

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <returns>�����</returns>
        string Receive();
    }
}